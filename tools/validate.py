"""Validate handbook links, bilingual coverage, assets, and source API contracts.

Python 3.10+, standard library only. This is not a Unity/gameplay test.
"""
from pathlib import Path
import hashlib
import json
import re
import struct
import sys
import zlib
from urllib.parse import unquote

ROOT = Path(__file__).resolve().parents[1]
checks = 0

def check(condition, message):
    global checks
    if not condition:
        raise AssertionError(message)
    checks += 1

def load_json(path):
    def no_duplicates(pairs):
        d = {}
        for key, value in pairs:
            check(key not in d, f'{path}: duplicate JSON key {key}')
            d[key] = value
        return d
    return json.loads(path.read_text(encoding='utf-8-sig'), object_pairs_hook=no_duplicates)

def png(path):
    data = path.read_bytes()
    check(data[:8] == b'\x89PNG\r\n\x1a\n', f'{path}: PNG signature')
    width, height = struct.unpack('>II', data[16:24])
    check(width > 0 and height > 0, f'{path}: PNG dimensions')
    pos, image_data = 8, bytearray()
    while pos < len(data):
        length = struct.unpack('>I', data[pos:pos+4])[0]
        kind, payload = data[pos+4:pos+8], data[pos+8:pos+8+length]
        crc = struct.unpack('>I', data[pos+8+length:pos+12+length])[0]
        check(zlib.crc32(kind+payload) & 0xffffffff == crc, f'{path}: corrupt PNG chunk')
        if kind == b'IDAT':
            image_data.extend(payload)
        pos += 12 + length
        if kind == b'IEND':
            break
    check(bool(zlib.decompress(image_data)), f'{path}: invalid compressed image')
    return width, height

def split_types(text):
    depth, start, parts = 0, 0, []
    for i, char in enumerate(text):
        if char in '<([': depth += 1
        elif char in '>)]': depth -= 1
        elif char == ',' and depth == 0:
            parts.append(text[start:i].strip()); start = i+1
    parts.append(text[start:].strip())
    return parts

def callback_contracts():
    contracts = {}
    for path in (ROOT/'src/API').rglob('*Builder.cs'):
        text = path.read_text(encoding='utf-8-sig')
        for match in re.finditer(r'public\s+\w+\s+(\w+)\((Action|Func)<', text):
            start, depth, end = match.end(), 1, match.end()
            while depth:
                if text[end] == '<': depth += 1
                elif text[end] == '>': depth -= 1
                end += 1
            count = len(split_types(text[start:end-1])) - (match.group(2) == 'Func')
            contracts[(path.stem, match.group(1))] = count
    return contracts

def validate():
    source = load_json(ROOT/'reference/source-manifest.json')
    actual_paths = {p.relative_to(ROOT/'src').as_posix() for p in (ROOT/'src').rglob('*') if p.is_file()}
    check(actual_paths == set(source), 'source file inventory drift')
    for rel, expected in source.items():
        digest = hashlib.sha256((ROOT/'src'/rel).read_bytes().replace(b'\r\n',b'\n')).hexdigest()
        check(digest == expected, f'source drift: {rel}')

    zh = {p.name for p in (ROOT/'docs/zh').glob('*.md')}
    en = {p.name for p in (ROOT/'docs/en').glob('*.md')}
    check(zh == en and len(zh) >= 10, 'bilingual chapter parity')
    for path in list((ROOT/'docs').rglob('*.md')) + list((ROOT/'example').rglob('*.md')) + list((ROOT/'reference').glob('*.md')) + [ROOT/'README.md']:
        text = path.read_text(encoding='utf-8-sig')
        check(len(re.findall(r'^```', text, re.M)) % 2 == 0, f'{path}: unclosed code fence')
        for target in re.findall(r'\]\(([^)]+)\)', text):
            if re.match(r'\w+://|mailto:|#', target): continue
            target = unquote(target.split('#',1)[0].strip('<>'))
            check((path.parent/target).exists(), f'{path}: missing link {target}')

    contracts = callback_contracts()
    mod_ids = set()
    mods = sorted((ROOT/'example/mods').iterdir())
    check(len(mods) == 7, 'seven example folders')
    for mod in mods:
        manifest = load_json(mod/'modinfo.json')
        for key in ['modID','name','version','author','description']:
            check(isinstance(manifest.get(key), str) and bool(manifest[key]), f'{mod}: missing {key}')
        check(manifest['modID'] not in mod_ids, f'{mod}: duplicate modID')
        mod_ids.add(manifest['modID'])
        check('workshopId' not in manifest, f'{mod}: do not ship someone else\'s item ID')
        check((mod/'README.md').is_file(), f'{mod}: missing bilingual readme')
        png(mod/'img.png')
        for path in mod.rglob('*.png'):
            png(path)
        scripts = list(mod.rglob('*.lua'))
        text = '\n'.join(p.read_text(encoding='utf-8') for p in scripts)
        if scripts:
            entry = (mod/'script/init.lua').read_text(encoding='utf-8')
            check(len(re.findall(r'^function init\(\)', entry, re.M)) == 1, f'{mod}: one global init')
            check('CONSOLE_ENABLED' not in text, f'{mod}: release example enables console')
        for module in re.findall(r'require\("([^"]+)"\)', text):
            check(module.startswith(manifest['modID']+'.'), f'{mod}: unnamespaced module {module}')
            check((mod/'script'/(module.replace('.','/')+'.lua')).exists(), f'{mod}: missing module')
        required = set()
        for builder_name, id in re.findall(r'(LuaArtifactBuilder|LuaTileMaterialBuilder|LuaMaterialSetBuilder)\.Create\("([^"]+)"', text):
            if builder_name == 'LuaArtifactBuilder':
                check(id.startswith(manifest['modID']+':'), f'{mod}: artifact namespace')
                required.update([f'artifact_{id}_name', f'artifact_{id}_description'])
                check((mod/'texture/artifact'/(id.split(':',1)[1]+'.png')).exists(), f'{mod}: missing artifact sprite')
            elif builder_name == 'LuaTileMaterialBuilder':
                check(id.startswith(manifest['modID']+':') and '_material' not in id, f'{mod}: unsafe material namespace')
                required.update([f'tile_{id}_material_name', f'tile_{id}_material_description', f'tile_{id}_material_name_short'])
                check((mod/'texture/tile_material'/(id.split(':',1)[1]+'.png')).exists(), f'{mod}: missing material sprite')
            else:
                required.add(f'material_set_{id}_name')
        for id in re.findall(r'RegisterCustomYaku\(\s*"([^"]+)"', text):
            for suffix in ['name','description','romaji_name']:
                required.add(f'yaku_custom_yaku:{id}_{suffix}')
        required.update(re.findall(r'SimpleEffect\("([^"]+)"', text))
        if required:
            lang_zh = load_json(mod/'lang/zh-CN.json')
            lang_en = load_json(mod/'lang/en-US.json')
            check(set(lang_zh) == set(lang_en), f'{mod}: translation key parity')
            check(required <= set(lang_zh), f'{mod}: missing translations {required-set(lang_zh)}')
            for key in required:
                check(bool(lang_zh[key]) and bool(lang_en[key]), f'{mod}: empty translation {key}')
                check(re.findall(r'%[.\d]*[a-z]',lang_zh[key]) == re.findall(r'%[.\d]*[a-z]',lang_en[key]), f'{mod}: format placeholder mismatch')
        # Track the enclosing builder chain; check callback arity against real C# declarations.
        for chain in re.finditer(r'(LuaArtifactBuilder|LuaTileMaterialBuilder|LuaMaterialSetBuilder)\.Create\([\s\S]*?:BuildAndRegister(?:Craftable)?\(\)', text):
            kind = chain.group(1)
            for callback in re.finditer(r':(\w+)\(function\(([^)]*)\)', chain.group()):
                name, args = callback.groups()
                check((kind,name) in contracts, f'{mod}: unknown callback {kind}.{name}')
                actual = len([a for a in args.split(',') if a.strip()])
                check(actual == contracts[kind,name], f'{mod}: {name} has {actual} args; source requires {contracts[kind,name]}')
        pack = mod/'texture-pack.json'
        if pack.exists():
            data = load_json(pack)
            check(data['formatVersion']==1 and len(data['replacements'])>0, f'{mod}: texture manifest version')
            for replacement in data['replacements']:
                image = (mod/replacement['file']).resolve()
                check(image.is_relative_to(mod.resolve()) and image.is_file(), f'{mod}: image path escapes or is missing')
                check(replacement['target']=='artifact:aotenjo:origami_bear', f'{mod}: unexpected sample target')
                check(replacement.get('filterMode')=='Point', f'{mod}: pixel filter')
                png(image)
    print(f'PASS {checks} static checks; {len(mods)} mods; {len(zh)} bilingual chapter pairs; source-backed callback signatures.')

if __name__ == '__main__':
    try:
        validate()
    except (AssertionError, ValueError, OSError) as error:
        print(f'FAIL: {error}', file=sys.stderr)
        sys.exit(1)
