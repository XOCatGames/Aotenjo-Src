# 用模组添加语言

[目录](README.md) · [English](../en/localization.md)

下面以**添加巴西葡萄牙语**为例；添加其他语言时，替换文件名和示例译文即可。

1. 在你的模组目录下创建 `lang/pt-BR.json`。
2. 用 UTF-8 保存翻译，格式为“文本键 → 译文”：

   ```json
   {
     "ui_StartNewGame": "Novo jogo",
     "ui_settings": "Configurações",
     "ui_Quit": "Sair"
   }
   ```

3. 启用模组并重启游戏，在语言选择界面选中新增语言，再按提示重启。

无需 Lua 注册。新语言中未填写的文本使用英文。

常用语言文件如下，均放在 `lang/` 下；同一行的文件名任选其一。其他语言及地区代码见[完整对照表（341 项）](../../reference/language-codes.md)。

| 语言 | 文件名 | 游戏语言代码 |
| --- | --- | --- |
| 简体中文 | `zh-CN.json` / `zh-Hans.json` / `zh-SG.json` | `zh-Hans` |
| 繁体中文 | `zh-TW.json` / `zh-Hant.json` / `zh-HK.json` / `zh-MO.json` | `zh-Hant` |
| 英语 | `en-US.json` / `en.json` | `en` |
| 日语 | `ja-JP.json` / `ja.json` | `ja` |
| 韩语 | `ko-KR.json` | `ko-KR` |
| 德语 | `de-DE.json` | `de-DE` |
| 法语 | `fr-FR.json` | `fr-FR` |
| 西班牙语 | `es-ES.json` | `es-ES` |
| 俄语 | `ru-RU.json` | `ru-RU` |
| 葡萄牙语（巴西） | `pt-BR.json` | `pt-BR` |
| 葡萄牙语（葡萄牙） | `pt-PT.json` | `pt-PT` |
| 意大利语 | `it-IT.json` | `it-IT` |
| 波兰语 | `pl-PL.json` | `pl-PL` |
| 乌克兰语 | `uk-UA.json` | `uk-UA` |
| 土耳其语 | `tr-TR.json` | `tr-TR` |
| 泰语 | `th-TH.json` | `th-TH` |
| 越南语 | `vi-VN.json` | `vi-VN` |
| 印度尼西亚语 | `id-ID.json` | `id-ID` |
