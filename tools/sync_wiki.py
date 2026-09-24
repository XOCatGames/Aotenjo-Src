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
    ('2.-Custom-Artifact-自定义遗物', 'artifacts', '自定义遗物 / Custom artifacts'),
    ('3.-Custom-Patterns-自定义番种', 'yakus', '自定义番种 / Custom yakus'),
    ('4.-Custom-Tile-Materials-自定义牌体', 'materials', '牌材质与材质组 / Tile materials and sets'),
    ('5.-Lua-Bridge-Lua与游戏对象', 'lua-bridge', 'Lua 与游戏对象 / Lua bridge'),
    ('6.-Recipes-自定义合成', 'recipes', '合成配方 / Crafting recipes'),
    ('7.-Texture-Packs-材质包', 'textures', '材质包与配色 / Texture packs and colors'),
    ('8.-Assets-and-Localization-图片与本地化', 'assets', '图片与本地化 / Assets and localization'),
    ('9.-Cookbook-玩法食谱', 'cookbook', '更多玩法示例 / Gameplay cookbook'),
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
    banner = (f'> 本页与[中英双语模组教程]({WEB}/blob/main/README.md)同步更新。想了解参考的源码和已经做过的检查，可以查看 '
              f'[Source]({WEB}/blob/main/docs/SOURCE.md) / [Validation]({WEB}/blob/main/docs/VALIDATION.md)。\n'
              '> Mirrored from the current bilingual handbook; source baseline and test scope are linked above.\n\n')
    pages = {name: banner + pair(chapter) for name, chapter, _ in CHAPTERS}
    pages['EX.0-Upload-to-Steam-Workshop-上传至创意工坊'] = banner + (
        section('docs/zh/testing.md', '5. 打包并上传创意工坊') + '\n\n'
        + section('docs/en/testing.md', 'ZIP packages and Workshop')
        + f'\n\n发布前，记得先按[示例测试步骤]({WEB}/blob/main/docs/zh/testing.md)在游戏里试一遍。 '
          f'Complete the [acceptance matrix]({WEB}/blob/main/docs/en/testing.md) before publishing.')
    pages['EX.1-LuaArtifactBuilder'] = banner + (
        f'[中文用法与回调说明]({WEB}/blob/main/docs/zh/artifacts.md) · '
        f'[English usage and callbacks]({WEB}/blob/main/docs/en/artifacts.md)\n\n'
        + section('reference/api-signatures.md', 'LuaArtifactBuilder'))
    for legacy, api in [('EX.2-Player', 'Player'), ('EX.5-Tile', 'Tile')]:
        pages[legacy] = banner + (
            f'[中文方法说明与玩法示例]({WEB}/blob/main/docs/zh/cookbook.md) · '
            f'[English object guide and recipes]({WEB}/blob/main/docs/en/cookbook.md)\n\n'
            '下面整理了 C# 中的公开方法，方便查询参数。部分方法在不同平台上的 Lua/AOT 绑定可能不同，建议先从教程演示过的接口开始。\n\n'
            'Public C# declarations do not guarantee Lua/AOT bindings on every platform; start with the demonstrated APIs.\n\n'
            + section('reference/api-signatures.md', api))
    pages['EX.3-Pattern-IDs-番种ID表速查'] = banner + read('reference/catalogs.md')
    pages['EX.4-Commands-指令'] = banner + (
        section('docs/zh/testing.md', '2. 开启测试控制台') + '\n\n'
        + section('docs/en/testing.md', 'Enable a test console'))

    navigation = '\n'.join(f'- [{label}]({wiki_url(name)})' for name, _, label in CHAPTERS)
    pages['Home'] = f'''# Aotenjo 模组手册 / Modding handbook

欢迎，各位挖井人！这份教程会从新建模组文件夹开始，带大家添加自己的遗物、番种、牌材质和合成配方，也可以给喜欢的物品换张图片、给牌面换一套颜色。了解 Lua 基本语法，就可以跟着下面的示例动手做。

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

教程参考当前源码编写，你在 Steam 安装的游戏可能还没有其中的新功能。可以先运行 Hello，再逐个尝试其他示例。目前 Boss、小道具、地点等内容还没有完整的 Lua Builder，需要相应接口后才能按教程的方式制作。旧 Wiki 链接也可以继续使用，里面已经更新成这版教程。

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
