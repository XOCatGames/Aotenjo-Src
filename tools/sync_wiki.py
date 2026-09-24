"""Refresh the checked-out public Wiki from the canonical bilingual handbook.

Writes the named pages only, preserving legacy Wiki URLs. Does not commit or push.
"""
import argparse
from pathlib import Path
import re
from urllib.parse import quote

ROOT = Path(__file__).resolve().parents[1]
WEB = 'https://github.com/XOCatGames/Aotenjo-Src'
CHAPTERS = [
    ('1.-Getting-Started-快速上手', 'quickstart', '快速上手 / Getting started'),
    ('2.-Custom-Artifact-自定义遗物', 'artifacts', '自定义藏品 / Custom artifacts'),
    ('3.-Custom-Patterns-自定义番种', 'yakus', '自定义番种 / Custom yakus'),
    ('4.-Custom-Tile-Materials-自定义牌体', 'materials', '牌材质与材质组 / Tile materials and sets'),
    ('5.-Lua-Bridge-Lua与游戏对象', 'lua-bridge', 'Lua 与游戏对象 / Lua bridge'),
    ('6.-Recipes-自定义合成', 'recipes', '合成配方 / Crafting recipes'),
    ('7.-Texture-Packs-材质包', 'textures', '材质包与配色 / Texture packs and colors'),
    ('8.-Assets-and-Localization-图片与本地化', 'assets', '图片与本地化 / Assets and localization'),
    ('9.-Cookbook-玩法食谱', 'cookbook', '玩法食谱 / Gameplay cookbook'),
    ('10.-Testing-and-Publishing-测试与发布', 'testing', '测试与发布 / Testing and publishing'),
]


def absolute_links(text, source):
    def replace(match):
        target = match.group(1)
        if re.match(r'\w+://|mailto:|#', target):
            return match.group(0)
        path, marker, anchor = target.partition('#')
        resolved = (source.parent / path).resolve()
        if not resolved.exists():
            raise ValueError(f'Missing link in {source}: {target}')
        rel = resolved.relative_to(ROOT).as_posix()
        return '](' + WEB + '/blob/main/' + quote(rel, safe='/') + (marker + anchor if marker else '') + ')'
    return re.sub(r'\]\(([^)]+)\)', replace, text)


def read(relative):
    path = ROOT / relative
    return absolute_links(path.read_text(encoding='utf-8-sig').strip(), path)


def pair(chapter):
    return ('## 中文\n\n' + read(f'docs/zh/{chapter}.md')
            + '\n\n---\n\n## English\n\n' + read(f'docs/en/{chapter}.md'))


def section(relative, title):
    text = read(relative)
    match = re.search(r'^## ' + re.escape(title) + r'\n[\s\S]*?(?=^## |\Z)', text, re.M)
    if not match:
        raise ValueError(f'Missing section: {relative}: {title}')
    return match.group().strip()


def wiki_url(name):
    return WEB + '/wiki/' + quote(name, safe='.-')


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--wiki-root', type=Path, required=True)
    args = parser.parse_args()
    wiki = args.wiki_root.resolve()
    if not (wiki / '.git').exists():
        raise ValueError('--wiki-root must be the existing Wiki Git checkout')
    banner = (f'> 本页同步自[中英双语模组手册]({WEB}/blob/main/README.md)，源码基准和验证边界见 '
              f'[Source]({WEB}/blob/main/docs/SOURCE.md) / [Validation]({WEB}/blob/main/docs/VALIDATION.md)。\n'
              '> Mirrored from the current bilingual handbook; source baseline and test scope are linked above.\n\n')
    pages = {name: banner + pair(chapter) for name, chapter, _ in CHAPTERS}
    pages['EX.0-Upload-to-Steam-Workshop-上传至创意工坊'] = banner + (
        section('docs/zh/testing.md', '分享安装包与 Workshop') + '\n\n'
        + section('docs/en/testing.md', 'ZIP packages and Workshop')
        + f'\n\n先完成[七个示例验收表]({WEB}/blob/main/docs/zh/testing.md)。 '
          f'Complete the [acceptance matrix]({WEB}/blob/main/docs/en/testing.md) before publishing.')
    pages['EX.1-LuaArtifactBuilder'] = banner + (
        f'[中文用法与回调说明]({WEB}/blob/main/docs/zh/artifacts.md) · '
        f'[English usage and callbacks]({WEB}/blob/main/docs/en/artifacts.md)\n\n'
        + section('reference/api-signatures.md', 'LuaArtifactBuilder'))
    for legacy, api in [('EX.2-Player', 'Player'), ('EX.5-Tile', 'Tile')]:
        pages[legacy] = banner + (
            f'[中文对象速查与食谱]({WEB}/blob/main/docs/zh/cookbook.md) · '
            f'[English object guide and recipes]({WEB}/blob/main/docs/en/cookbook.md)\n\n'
            'C# 公开声明不等于每个平台都已生成 Lua/AOT 绑定；优先使用教程演示的接口。\n\n'
            'Public C# declarations do not guarantee Lua/AOT bindings on every platform; start with the demonstrated APIs.\n\n'
            + section('reference/api-signatures.md', api))
    pages['EX.3-Pattern-IDs-番种ID表速查'] = banner + read('reference/catalogs.md')
    pages['EX.4-Commands-指令'] = banner + (
        section('docs/zh/testing.md', '开启测试控制台') + '\n\n'
        + section('docs/en/testing.md', 'Enable a test console'))

    navigation = '\n'.join(f'- [{label}]({wiki_url(name)})' for name, _, label in CHAPTERS)
    pages['Home'] = f'''# Aotenjo 模组手册 / Modding handbook

只需要 Lua 基本语法，从第一个可运行模组到自定义藏品、番种、牌材质、合成、材质包与配色。

Start with basic Lua syntax and build custom artifacts, yakus, tile materials, crafting recipes, texture packs, and face colors.

**[中文完整手册]({WEB}/blob/main/docs/zh/README.md) · [English handbook]({WEB}/blob/main/docs/en/README.md)**

**[7 个完整示例 / Seven complete examples]({WEB}/blob/main/example/README.md) · [下载全部 / Download ZIP]({WEB}/raw/refs/heads/main/downloads/all-examples.zip)**

{navigation}

## 查询 / Reference

- [API 签名与源码 / API signatures and source]({WEB}/blob/main/reference/api-signatures.md)
- [事件 / Events]({WEB}/blob/main/reference/events.md)
- [番种包、番种、材质与字体 ID / ID catalogs]({WEB}/blob/main/reference/catalogs.md)
- [贴图目标目录 / Texture target catalog]({WEB}/blob/main/reference/textures.csv)
- [源码基准 / Source baseline]({WEB}/blob/main/docs/SOURCE.md)
- [测试结果与实机验收边界 / Validation scope]({WEB}/blob/main/docs/VALIDATION.md)

本手册按当前源码重写。源码快照不是 Steam 发行版本号；先在目标游戏版本测试 Hello，再逐个增加示例。尚无完整 Lua Builder 的 Boss、小道具、地点等功能不列为已支持。旧 Wiki 地址保留，内容已与新手册同步。

Rewritten against the current source snapshot, which is not a Steam release version. Test Hello on your installed build, then add examples one at a time. Bosses, gadgets, and locations without complete Lua builders are not advertised as supported. Legacy Wiki URLs remain available with updated content.
'''
    pages['_Sidebar'] = f'[首页 / Home]({WEB}/wiki)\n\n' + navigation + (
        f'\n\n[完整示例 / Examples]({WEB}/blob/main/example/README.md)\n\n'
        f'[API reference]({WEB}/blob/main/reference/api-signatures.md)')
    pages['_Footer'] = (
        f'[中文]({WEB}/blob/main/docs/zh/README.md) · [English]({WEB}/blob/main/docs/en/README.md) · '
        f'[Edit canonical docs]({WEB}/tree/main/docs) · '
        '`python tools/sync_wiki.py --wiki-root ../Aotenjo-Src.wiki`')
    for name, text in pages.items():
        (wiki / (name + '.md')).write_text(text.strip() + '\n', encoding='utf-8', newline='\n')
    print(f'PASS {len(pages)} Wiki pages refreshed; all relative links resolved to public source files.')


if __name__ == '__main__':
    main()
