using Sirenix.OdinInspector;

namespace Aotenjo
{
    public enum YakuType
    {
        Base,

        //顺子系

        [LabelText("一般高")]
        YiBanGao,

        [LabelText("连六")]
        LianLiu,

        [LabelText("喜相逢")]
        //喜相逢
        XiXiangFeng,

        [LabelText("老少副")]
        //老少副
        LaoShaoFu,

        [LabelText("平和")]
        //平和
        PingHu,

        [LabelText("三色三步高")]
        //三色三步高
        SanSeSanBuGao,

        [LabelText("三色三同顺")]
        //三色三同顺
        SanSeSanTongShun,

        [LabelText("一色三同顺")]
        //一色三同顺
        YiSeSanTongShun,

        [LabelText("一色四同顺")]
        //一色四同顺
        YiSeSiTongShun,

        [LabelText("一色三步高")]
        //一色三步高
        YiSeSanBuGao,

        [LabelText("花龙")]
        //花龙
        HuaLong,

        [LabelText("清龙")]
        //清龙
        QingLong,

        [LabelText("三色双龙会")]
        //三色双龙会
        SanSeShuangLongHui,

        [LabelText("一色双龙会")]
        //一色双龙会
        YiSeShuangLongHui,

        [LabelText("连七对")]
        //连七对
        LianQiDui,

        //数系

        [LabelText("断幺")]
        //断幺
        DuanYao,

        [LabelText("全带幺")]
        //全带幺
        QuanDaiYao,

        [LabelText("全带五")]
        //全带五
        QuanDaiWu,

        [LabelText("大于五")]
        //大于五
        DaYuWu,

        [LabelText("小于五")]
        //小于五
        XiaoYuWu,

        [LabelText("全大")]
        //全大
        QuanDa,

        [LabelText("全中")]
        //全中
        QuanZhong,

        [LabelText("全小")]
        //全小
        QuanXiao,

        [LabelText("推不倒")]
        //推不倒
        TuiBuDao,

        //花色系

        [LabelText("清一色")]
        //清一色
        QingYiSe,

        [LabelText("混一色")]
        //混一色
        HunYiSe,

        [LabelText("字一色")]
        //字一色
        ZiYiSe,

        [LabelText("五门齐")]
        //五门齐
        WuMenQi,

        [LabelText("缺一门")]
        //缺一门
        QueYiMen,

        [LabelText("无字")]
        //无字
        WuZi,

        [LabelText("绿一色")]
        //绿一色
        LyuYiSe,

        [LabelText("九莲宝灯")]
        //九莲宝灯
        JiuLianBaoDeng,

        //刻子系

        [LabelText("箭刻")]
        //箭刻
        JianKe,

        [LabelText("门风刻")]
        //门风刻
        MenFengKe,

        [LabelText("圈风刻")]
        //圈风刻
        QuanFengKe,

        [LabelText("三风刻")]
        //三风刻
        SanFengKe,

        [LabelText("双箭刻")]
        //双箭刻
        ShuangJianKe,

        [LabelText("混幺九")]
        //混幺九
        HunYaoJiu,

        [LabelText("小三元")]
        //小三元
        XiaoSanYuan,

        [LabelText("小四喜")]
        //小四喜
        XiaoSiXi,

        [LabelText("双同字刻")]
        //双同字刻
        ShuangTongZiKe,

        [LabelText("大三元")]
        //大三元
        DaSanYuan,

        [LabelText("大四喜")]
        //大四喜
        DaSiXi,

        [LabelText("清幺九")]
        //清幺九
        QingYaoJiu,

        [LabelText("三同字刻")]
        //三同字刻
        SanTongZiKe,

        [LabelText("四同字刻")]
        //四同字刻
        SiTongZiKe,

        [LabelText("一色三节高")]
        //一色三节高
        YiSeSanJieGao,

        [LabelText("一色四节高")]
        //一色四节高
        YiSeSiJieGao,

        //山包番种

        [LabelText("双刻")]
        //双刻
        ShuangKe,

        [LabelText("三刻")]
        //三刻
        SanKe,

        [LabelText("四刻")]
        //四刻
        SiKe,

        [LabelText("四归一")]
        //四归一
        SiGuiYi,

        [LabelText("五归一")]
        //五归一
        WuGuiYi,

        [LabelText("多归一")]
        //多归一
        DuoGuiYi,

        [LabelText("双色双同刻")]
        //双色双同刻
        ShuangSeShuangTongKe,

        [LabelText("三色三同刻")]
        //三色三同刻
        SanSeSanTongKe,

        [LabelText("全双刻")]
        //全双刻
        QuanShuangKe,

        [LabelText("一色双同刻")]
        //一色双同刻
        YiSeShuangTongKe,

        [LabelText("一色三同刻")]
        //一色三同刻
        YiSeSanTongKe,

        [LabelText("一色四同刻")]
        //一色四同刻
        YiSeSiTongKe,

        //6.3新增

        [LabelText("一色四步高")]
        //一色四步高
        YiSeSiBuGao,

        [LabelText("镜同顺")]
        //镜同顺
        JingTongShun,

        [LabelText("镜同刻")]
        //镜同刻
        JingTongKe,

        [LabelText("百万石")]
        //百万石
        BaiWanShi,

        //6.15新增

        [LabelText("杠")]
        //杠
        Gang,

        [LabelText("双杠")]
        //双杠
        ShuangGang,

        [LabelText("三杠")]
        //三杠
        SanGang,

        [LabelText("四杠")]
        //四杠
        SiGang,

        [LabelText("两般高")]
        //两般高
        LiangBanGao,

        [LabelText("七对")]
        //七对
        QiDui,
        
        [LabelText("十三幺")]
        ShiSanYao,

        [LabelText("三色三节高")]
        //三色三节高
        SanSeSanJieGao,

        [LabelText("断红")]
        //断红
        DuanHong,

        //9.8新增
        [LabelText("金门桥")]
        JinMenQiao,
        [LabelText("七星对")]
        QiXingDui,
        [LabelText("三元对")]
        SanYuanDui,
        [LabelText("四喜对")]
        SiXiDui,
        [LabelText("跳牌对")]
        TiaoPaiKe,
        [LabelText("筋牌对")]
        JinPaiKe,
        [LabelText("顶三刻")]
        DinSanKe,
        [LabelText("将对")]
        JiangDui,
        [LabelText("三色跳牌刻")]
        SanSeTiaoPaiKe,
        [LabelText("三色筋牌刻")]
        SanSeJinPaiKe,
        [LabelText("三色顶三刻")]
        SanSeDinSanKe,
        [LabelText("四跳牌刻")]
        SiTiaoPaiKe,
        [LabelText("大车轮")]
        DaCheLun,
        [LabelText("小车轮")]
        XiaoCheLun,
        [LabelText("大竹林")]
        DaZhuLin,
        [LabelText("小竹林")]
        XiaoZhuLin,
        [LabelText("大数邻")]
        DaShuLin,
        [LabelText("小数邻")]
        XiaoShuLin,
        [LabelText("纯全带幺")]
        ChunQuanDaiYao,
        [LabelText("青洞门")]
        QingDongMen,
        [LabelText("红孔雀")]
        HongKongQue,

        [LabelText("天地创造")]
        TianDiChuangZao,

        //银河番种
        [LabelText("双同字顺")]
        ShuangTongZiShun,
        [LabelText("三同字顺")]
        SanTongZiShun,
        [LabelText("四同字顺")]
        SiTongZiShun,
        [LabelText("四喜顺")]
        SiFengShun,
        [LabelText("同七对")]
        QiTongDui,

        //五彩番种
        [LabelText("正花")]
        ZhengHua,
        [LabelText("一台花")]
        YiTaiHua,
        [LabelText("八仙过海")]
        BaXianGuoHai,

        [LabelText("全单")]
        QuanDan,

        //隐藏番种
        [LabelText("无番和")]
        WuFanHu,

        //阴阳番种
        [LabelText("阴阳顺")]
        YinYangShun,
        [LabelText("阴扣")]
        YinKou,
        [LabelText("阴阳龙")]
        YinYangLong,
        [LabelText("奈何桥")]
        NaiHeQiao,
        [LabelText("两界桥")]
        LiangJieQiao,
        [LabelText("阴阳三步高")]
        YinYangSanBuGao,
        [LabelText("阴阳两般高")]
        YinYangLiangBanGao,

        [LabelText("龙七对")]
        LongQiDui,
        [LabelText("双龙七对")]
        ShuangLongQiDui,
        [LabelText("三龙七对")]
        SanLongQiDui
    }
}