# Add a language with a mod

[Contents](README.md) · [中文](../zh/localization.md)

This example adds **Brazilian Portuguese**. To add another language, replace the filename and example translations.

1. Create `lang/pt-BR.json` inside your mod folder.
2. Save the translations as UTF-8, mapping text keys to translated strings:

   ```json
   {
     "ui_StartNewGame": "Novo jogo",
     "ui_settings": "Configurações",
     "ui_Quit": "Sair"
   }
   ```

3. Enable the mod and restart the game. Choose the new language in the language selector, then follow the restart prompt.

No Lua registration is needed. Missing translations in the new language use English.

Common language files are listed below. Place them in `lang/` and choose one filename per row. See the [complete table (341 entries)](../../reference/language-codes.md) for all language and region codes.

| Language | Filename | Game language code |
| --- | --- | --- |
| Simplified Chinese | `zh-CN.json` / `zh-Hans.json` / `zh-SG.json` | `zh-Hans` |
| Traditional Chinese | `zh-TW.json` / `zh-Hant.json` / `zh-HK.json` / `zh-MO.json` | `zh-Hant` |
| English | `en-US.json` / `en.json` | `en` |
| Japanese | `ja-JP.json` / `ja.json` | `ja` |
| Korean | `ko-KR.json` | `ko-KR` |
| German | `de-DE.json` | `de-DE` |
| French | `fr-FR.json` | `fr-FR` |
| Spanish | `es-ES.json` | `es-ES` |
| Russian | `ru-RU.json` | `ru-RU` |
| Portuguese (Brazil) | `pt-BR.json` | `pt-BR` |
| Portuguese (Portugal) | `pt-PT.json` | `pt-PT` |
| Italian | `it-IT.json` | `it-IT` |
| Polish | `pl-PL.json` | `pl-PL` |
| Ukrainian | `uk-UA.json` | `uk-UA` |
| Turkish | `tr-TR.json` | `tr-TR` |
| Thai | `th-TH.json` | `th-TH` |
| Vietnamese | `vi-VN.json` | `vi-VN` |
| Indonesian | `id-ID.json` | `id-ID` |
