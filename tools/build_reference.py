"""Generate source-backed reference indexes. No Unity assets are redistributed.

python tools/build_reference.py --unity-root ../Aotenjo-Unity
Only maintainers need the Unity checkout; readers use the generated references.
"""
import argparse
import csv
import hashlib
import json
from pathlib import Path
import re
import struct

ROOT = Path(__file__).resolve().parents[1]
REF = ROOT / 'reference'

def normalized_hash(path):
    return hashlib.sha256(path.read_bytes().replace(b'\r\n', b'\n')).hexdigest()

def declarations(text):
    # Declaration index, not a C# parser: intentionally retains exact source text.
    lines = text.splitlines()
    for i, line in enumerate(lines):
        line = line.split('//', 1)[0].strip()
        line = re.sub(r'^(?:\[[^\]]*\]\s*)+', '', line)
        if not line.startswith('public ') or '(' not in line:
            continue
        candidates = list(re.finditer(r'\b([A-Za-z_]\w*)(?:<[^()]*>)?\s*\(', line))
        method = next((m for m in candidates if m.group(1) not in {
            'public', 'static', 'override', 'virtual', 'abstract', 'new', 'sealed', 'async'}), None)
        if method is None:
            continue
        start = line.index('(', method.start())
        before = line[:start]
        if '=' in before or re.search(r'\b(class|delegate|event)\b', before):
            continue
        signature = line
        j = i
        while signature.count('(') > signature.count(')') and j + 1 < len(lines):
            j += 1
            signature += ' ' + lines[j].split('//', 1)[0].strip()
        # Stop at the balanced closing parenthesis, retaining tuple/generic types.
        depth = 0
        for end in range(start, len(signature)):
            if signature[end] == '(':
                depth += 1
            elif signature[end] == ')':
                depth -= 1
                if depth == 0:
                    yield i + 1, signature[:end + 1]
                    break

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--unity-root', type=Path, required=True)
    args = parser.parse_args()
    unity = args.unity_root.resolve()
    REF.mkdir(exist_ok=True)
    public = ROOT / 'src'
    snapshot = {str(p.relative_to(public)).replace('\\', '/'): normalized_hash(p)
                for p in sorted(public.rglob('*')) if p.is_file()}
    (REF / 'source-manifest.json').write_text(json.dumps(snapshot, indent=2) + '\n', encoding='utf-8')

    selected = list((public / 'API').rglob('*.cs'))
    selected += [public / s for s in [
        'Player/Player.cs', 'Player/SkillSet.cs', 'Artifact/Artifact.cs',
        'Artifact/Artifacts.cs', 'ArtifactRecipe/ArtifactRecipe.cs',
        'HandAndTile/Tile/Tile.cs', 'HandAndTile/Hand/Block/Block.cs',
        'HandAndTile/Hand/Permutation/Permutation.cs', 'HandAndTile/Yaku/YakuType.cs',
        'HandAndTile/Tile/TileProperties/TileMaterial/TileMaterial.cs',
        'HandAndTile/Tile/TileProperties/TileMaterial/MaterialSet/MaterialSet.cs',
        'Event/NewEventSystem/EventBus.cs', 'Utils/LotteryPool.cs']]
    selected += list((public / 'Effects').glob('*.cs'))
    out = ['# API 源码签名 / Source API signatures', '',
        '[中文手册](../docs/zh/README.md) · [English handbook](../docs/en/README.md)', '',
        '自动提取公开声明，用于精确查参数；构造函数也在其中。不是所有声明都有跨平台 Lua/AOT 绑定保证。',
        'Extracted public declarations, including constructors. This is a parameter lookup index, not a guarantee of Lua/AOT binding for every member.', '',
        'C# notation: `Action<A,B>` means a callback `(a,b)` with no return; `Func<A,B,R>` means `(a,b)` returning R. `List<T>` uses Count, arrays use Length; both start at 0.', '']
    for path in sorted(set(selected)):
        if not path.exists():
            # Block location has changed between source versions.
            hits = list(public.rglob(path.name))
            if not hits:
                raise FileNotFoundError(path)
            path = hits[0]
        text = path.read_text(encoding='utf-8-sig')
        ns = re.search(r'namespace\s+([\w.]+)', text)
        out += [f'## {path.stem}', '', f'Namespace: `{ns.group(1) if ns else "global / 全局"}` · [源码 / source](../src/{path.relative_to(public).as_posix()})', '', '```csharp']
        out.extend(s + ';' for _, s in declarations(text))
        out += ['```', '']
    (REF / 'api-signatures.md').write_text('\n'.join(out), encoding='utf-8')

    events = ['# 事件目录 / Event directory', '',
        '[事件使用边界 / Event limitations](../docs/en/cookbook.md) · [中文说明](../docs/zh/cookbook.md)', '',
        '按精确发布类型订阅；字段可能来自基类，请沿源码继承关系查看。',
        'Subscribe to the exact published type. Payload fields may be inherited; follow the linked base classes.', '',
        '| 类型 / Type declaration | 源码 / Source |', '| --- | --- |']
    for p in sorted((public / 'Event').rglob('*.cs')):
        for m in re.finditer(r'^\s*public\s+(?:(?:sealed|abstract|static)\s+)*class\s+([^\n{]+)', p.read_text(encoding='utf-8-sig'), re.M):
            events.append(f'| `{m.group(1).strip()}` | [{p.stem}](../src/{p.relative_to(public).as_posix()}) |')
    (REF / 'events.md').write_text('\n'.join(events) + '\n', encoding='utf-8')

    packs = json.loads((unity / 'Assets/Resources/Config/YakuPacks.json').read_text(encoding='utf-8-sig'))
    out = ['# ID 目录 / ID catalogs', '', '[中文番种教程](../docs/zh/yakus.md) · [English yaku guide](../docs/en/yakus.md)', '',
           '## 番种包 / Yaku packs', '',
           '来自当前 YakuPacks.json 的列表顺序；API 接受 index，不能把显示名称当索引。',
           'List order from the current YakuPacks.json. The API takes the index, not the display name.', '',
           '| index | id | name |', '| --- | --- | --- |']
    out += [f'| {i} | {p["id"]} | `{p["name"]}` |' for i, p in enumerate(packs)]
    out += ['', '## 内置番种 / Built-in yakus', '',
            '传入枚举名称，区分拼写；CustomYaku 是占位标记，不是一个具体自定义番种。',
            'Use the enum spelling. CustomYaku is a marker, not a concrete custom yaku.', '',
            '[源枚举 / Source enum](../src/HandAndTile/Yaku/FixedYakuType.cs)', '', '```text']
    enum = (public / 'HandAndTile/Yaku/FixedYakuType.cs').read_text(encoding='utf-8-sig')
    enum = re.sub(r'//[^\n]*|\[[^\]]*\]', '', enum)
    body = enum[enum.index('{', enum.index('enum ')) + 1:enum.rindex('}')]
    names = re.findall(r'\b(\w+)\s*(?:=\s*\d+)?\s*(?:,|(?=\}))', body)
    out += names + ['```', '', '## 常用材质组 / Built-in material sets', '',
        '`basic`, `ore`, `porcelain`, `monsters`, `woods`, `desserts`, `mech_parts`.', '',
        '[全部材质与查找规则 / Materials and lookup rules](../src/HandAndTile/Tile/TileProperties/TileMaterial/TileMaterial.cs)', '',
        '## 内置字体 / Built-in fonts', '', '`plain`, `blue`, `red`, `neon`, `colorless` (lookup also accepts `_font`).', '',
        '[字体源码 / Font source](../src/HandAndTile/Tile/TileProperties/TileFont/TileFont.cs)', '']
    (REF / 'catalogs.md').write_text('\n'.join(out), encoding='utf-8')

    with (REF / 'textures.csv').open('w', encoding='utf-8', newline='') as f:
        writer = csv.writer(f)
        writer.writerow(['resource_path', 'texture_name', 'source_width', 'source_height'])
        resource_root = unity / 'Assets/Resources'
        for p in sorted(resource_root.rglob('*.png')):
            data = p.read_bytes()[:24]
            if data[:8] != b'\x89PNG\r\n\x1a\n':
                raise ValueError(f'Invalid PNG: {p}')
            w, h = struct.unpack('>II', data[16:24])
            writer.writerow([p.relative_to(resource_root).with_suffix('').as_posix(), p.stem, w, h])

    runtime_files = ['Assets/Scripts/AotenjoModLoader/ModManager.cs',
        'Assets/Scripts/AotenjoModLoader/ModLocalizationLoader.cs',
        'Assets/Scripts/Tile/TileFaceMaterialRegistry.cs', 'Assets/Scripts/Tile/SpriteManager.cs',
        'Assets/Scripts/Console/ArgumentParsers.cs', 'Assets/Scripts/Console/AotenjoCommandSystem.cs',
        'Assets/Scripts/UI/UIConsole.cs', 'Assets/Scripts/Integration/SteamWorkshopIntegration.cs',
        'Assets/Scripts/UI/MainMenu/ModManager/UIMainMenuModManager.cs',
        'Assets/Resources/Config/YakuPacks.json']
    provenance = {'source_commit': '406dba6623ad528224eac65f5d894269907681a8',
        'upstream_commit': 'c66f4500de0d2d19d627cc9d2455832ba7ff66c4',
        'date': '2026-09-24', 'normalization': 'CRLF to LF, SHA-256',
        'working_tree_omissions': ['Assets/LogicScripts/Utils/Constants.cs'],
        'runtime_sources_reviewed': {s: normalized_hash(unity / s) for s in runtime_files},
        'source_files': len(snapshot)}
    (REF / 'provenance.json').write_text(json.dumps(provenance, indent=2) + '\n', encoding='utf-8')
    print(f'Indexed {len(snapshot)} source files, {len(packs)} packs, {len(names)} yaku enum values.')

if __name__ == '__main__':
    main()
