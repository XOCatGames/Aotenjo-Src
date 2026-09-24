# ID 目录 / ID catalogs

[中文番种教程](../docs/zh/yakus.md) · [English yaku guide](../docs/en/yakus.md)

## 番种包 / Yaku packs

来自当前 YakuPacks.json 的列表顺序；API 接受 index，不能把显示名称当索引。
List order from the current YakuPacks.json. The API takes the index, not the display name.

| index | id | name |
| --- | --- | --- |
| 0 | 0 | `wind` |
| 1 | 1 | `forest` |
| 2 | 2 | `fire` |
| 3 | 3 | `mountain` |
| 4 | 4 | `sanyuan` |
| 5 | 5 | `bugao` |
| 6 | 6 | `tongse` |
| 7 | 7 | `lianke` |
| 8 | 8 | `sifeng` |
| 9 | 9 | `lianshun` |
| 10 | 10 | `qimen` |
| 11 | 11 | `tongke` |
| 12 | 12 | `yaojiu` |
| 13 | 13 | `tongshun` |
| 14 | 14 | `jushu` |
| 15 | 15 | `jupai` |

## 内置番种 / Built-in yakus

传入枚举名称，区分拼写；CustomYaku 是占位标记，不是一个具体自定义番种。
Use the enum spelling. CustomYaku is a marker, not a concrete custom yaku.

[源枚举 / Source enum](../src/HandAndTile/Yaku/FixedYakuType.cs)

```text
Base
YiBanGao
LianLiu
XiXiangFeng
LaoShaoFu
PingHu
SanSeSanBuGao
SanSeSanTongShun
YiSeSanTongShun
YiSeSiTongShun
YiSeSanBuGao
HuaLong
QingLong
SanSeShuangLongHui
YiSeShuangLongHui
LianQiDui
DuanYao
QuanDaiYao
QuanDaiWu
DaYuWu
XiaoYuWu
QuanDa
QuanZhong
QuanXiao
TuiBuDao
QingYiSe
HunYiSe
ZiYiSe
WuMenQi
QueYiMen
WuZi
LyuYiSe
JiuLianBaoDeng
JianKe
MenFengKe
QuanFengKe
SanFengKe
ShuangJianKe
HunYaoJiu
XiaoSanYuan
XiaoSiXi
ShuangTongZiKe
DaSanYuan
DaSiXi
QingYaoJiu
SanTongZiKe
SiTongZiKe
YiSeSanJieGao
YiSeSiJieGao
ShuangKe
SanKe
SiKe
SiGuiYi
WuGuiYi
DuoGuiYi
ShuangSeShuangTongKe
SanSeSanTongKe
QuanShuangKe
YiSeShuangTongKe
YiSeSanTongKe
YiSeSiTongKe
YiSeSiBuGao
JingTongShun
JingTongKe
BaiWanShi
Gang
ShuangGang
SanGang
SiGang
LiangBanGao
QiDui
ShiSanYao
SanSeSanJieGao
DuanHong
JinMenQiao
QiXingDui
SanYuanDui
SiXiDui
TiaoPaiKe
JinPaiKe
DinSanKe
JiangDui
SanSeTiaoPaiKe
SanSeJinPaiKe
SanSeDinSanKe
SiTiaoPaiKe
DaCheLun
XiaoCheLun
DaZhuLin
XiaoZhuLin
DaShuLin
XiaoShuLin
ChunQuanDaiYao
QingDongMen
HongKongQue
TianDiChuangZao
ShuangTongZiShun
SanTongZiShun
SiTongZiShun
SiFengShun
QiTongDui
ZhengHua
YiTaiHua
BaXianGuoHai
QuanDan
WuFanHu
YinYangShun
YinKou
YinYangLong
NaiHeQiao
LiangJieQiao
YinYangSanBuGao
YinYangLiangBanGao
LongQiDui
ShuangLongQiDui
SanLongQiDui
CustomYaku
YiSeWuTongShun
YiSeWuTongKe
YiSeWuJieGao
YiSeWuBuGao
WuGang
WuKe
ZhenWuMenQi
ZhenJiuLianBaoDeng
ZhenYiSeShuangLongHui
ZhenSanSeShuangLongHui
WuTongZiKe
LiGuLiGu
YiShiSanYao
```

## 常用材质组 / Built-in material sets

`basic`, `ore`, `porcelain`, `monsters`, `woods`, `desserts`, `mech_parts`.

[全部材质与查找规则 / Materials and lookup rules](../src/HandAndTile/Tile/TileProperties/TileMaterial/TileMaterial.cs)

## 内置字体 / Built-in fonts

`plain`, `blue`, `red`, `neon`, `colorless` (lookup also accepts `_font`).

[字体源码 / Font source](../src/HandAndTile/Tile/TileProperties/TileFont/TileFont.cs)
