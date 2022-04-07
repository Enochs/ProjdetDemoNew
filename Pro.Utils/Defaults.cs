using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pro.Utils
{
    /// <summary>
    /// 默认值
    /// </summary>
    public class Defaults
    {
        /// <summary>
        /// 默认每页显示的记录数
        /// </summary>
        public const int GridPageSize = 20;
        public const int GridSubPageSize = 200;
        /// <summary>
        /// 默认当前显示第几页
        /// </summary>
        public const int GridPage = 1;

        public const string IsDel = "N";
        /// <summary>
        /// 自动auto(小写
        /// </summary>
        public const string Auto = "auto";
        /// <summary>
        /// 字符串Y
        /// </summary>
        public const string YesorNo_True = "Y";
        /// <summary>
        /// 字符串N
        /// </summary>
        public const string YesorNo_False = "N";
        /// <summary>
        /// 会员邮件 是+否
        /// </summary>
        public const string YesorNo_All = "A";
        /// <summary>
        /// sql null值排序，排前面
        /// </summary>
        public const string NullValueFirst = "FIRST";
        /// <summary>
        /// sql null值排序，排后面
        /// </summary>
        public const string NullValueLast = "LAST";

        #region 英文、中文匹配

        /// <summary>
        /// 英文
        /// </summary>
        public const int CNorEN_EN = 1;

        /// <summary>
        /// 纯中文
        /// </summary>
        public const int CNorEN_CN = 2;

        #endregion

        #region 邮件匹配状态
        /// <summary>
        /// 全自动匹配
        /// </summary>
        public const string Match_A = "A";
        /// <summary>
        /// 半自动匹配
        /// </summary>
        public const string Match_B = "B";
        /// <summary>
        /// 匹配失败
        /// </summary>
        public const string Match_E = "E";
        /// <summary>
        /// 未匹配
        /// </summary>
        public const string Match_S = "S";

        /// <summary>
        /// 警告：禁止用于赋值！！（用于搜索 邮件匹配 自动任务优化方案）已匹配的数据（“A,B,E”） 
        /// </summary>
        public const string Match_NS = "NS";
        #endregion

        /// <summary>
        /// 字符串 true
        /// </summary>
        public const string TrueorFalse_True = "true";

        /// <summary>
        /// 字符串 false
        /// </summary>
        public const string TrueorFalse_False = "false";

        /// <summary>
        /// 字符串 False
        /// </summary>
        public const string TrueOrFalse_False = "False";

        /// <summary>
        /// 是否查询缓慢数据
        /// </summary>
        public const bool Is_Search_Lazy_Data = true;
        /// <summary>
        /// 淘宝taobao（小写
        /// </summary>
        public const string TaoBao = "taobao";
        /// <summary>
        /// 阿里ali（小写
        /// </summary>
        public const string ALi = "ali";
        /// <summary>
        /// 无平台（小写
        /// </summary>
        public const string None = "none";

        public const string Status = "0";
        public const string No_Status = "1";
        public const string Type = "0";
        /// <summary>
        /// 正常，一般
        /// </summary>
        public const short StatusNormal = 0;
        /// <summary>
        /// 禁用,删除
        /// </summary>
        public const short StatusDisabled = 1;

        public const string DateFormat = "yyyy-MM-dd";
        public const string DateFormatHours = "yyyy-MM-dd HH:00:00";
        public const string DateFormatMinutes = "yyyy-MM-dd HH:mm:00";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string DateTimeFormatmin = "yyyy-MM-dd 00:00";
        public const string DateTimeFormatmax = "yyyy-MM-dd 23:59";
        public const string DateTimeFormatSecondMin = "yyyy-MM-dd 00:00:00";
        public const string DateTimeFormatSecondMax = "yyyy-MM-dd 23:59:59";
        public const string DateTimeFormatmaxs = "yyyy-MM-dd 11:59";
        // public const string DateTimeFormats1 = "yyyy-MM-dd HH:59:59";
        public const string DateFormatDot = "yyyy.MM.dd";
        public const string DateMonthFormat = "yyyy-MM";
        public const string DateTimeForMin = "yyyy-MM-dd HH:mm";
        public const string DateyyyyMMdd = "yyyyMMdd";
        public const string DATE_YYYY_MM_DD_HH_MM_SS_FFF = "yyyyMMddHHmmssfff";
        public const string DateFormatymd = "yyyy/MM/dd";
        public const string DateFormatZH_CN = "yyyy年MM月dd日";

        /// <summary>
        /// 日期格式
        /// </summary>
        public const string DateFormatymdHms = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        ///时间格式
        /// </summary>
        public const string DateFormatTime = "HH:mm:ss";

        /// <summary>
        ///时间格式[HH:MM]
        /// </summary>
        public const string DateHHmm = "HH:mm";

        /// <summary>
        /// StockChange表和StockChangeItem表保留数据时长，单位为月，如6个月
        /// </summary>
        public const int StockChangeSaveDuration = 6;

        public const string EBAYWCFPROXYKEY = "mXFG9hvcyo7wowqTNCbJMmINJ6RhYP7f";
        /// <summary>
        /// EBay刊登需要过滤的敏感词 
        /// </summary>
        public const string EBaySensitiveWords = "velcro,canon,swarovski,geneva,bluw,磁铁球,triangl,kapton,naked,lip ink,hot end,heavy grip,shamballa,baton,frog,kodi,moooi,swinger,yoga toes,graphics,luxottica,oakley,ray-ban,wayfarer,as seen on tv,bakery direct,blinkring,dremel,wallpaper,vw,magideal,dremel,cable drop,simply clean,beauty blender,plexiglas,kinesio,memobottle,amenitee,graco.volkswagen,vw,gti,harley davidson,hd,not,better,better than,joy-con,retekess,john emmons,rachel dews,aoshumofa,street fighter,quick,mask,scrabble,oral-b,sheing,trolleybags";
        /// <summary>
        /// Amazon刊登敏感词
        /// </summary>
        public const string AmazonSensitiveWords = "电子烟";
        public const string AmazonJPSensitiveWords = "无线,对讲机";
        public const string AmazonSensitiveDescribeWords = "batman,pokemon,laser,black & decker";
        /// <summary>
        /// 仓库APITOKEN
        /// </summary>
        public const string ImcWareHouseAPIToken = "l2bf5vpXK2uV9i3flcC5hqeTMIR1TnK/";

        /// <summary>
        /// 公共导出链接地址
        /// </summary>
        public const string ImcPublicImportUrl = "/Import/ImportExcel";

        /// <summary>
        /// 2019年时间字符串
        /// </summary>
        public const string DATETIME_2019 = "2019-01-01";

        /// <summary>
        /// 公司代码艾姆诗 亚马逊精品用
        /// </summary>
        public const string CompanyCode_AMS = "艾姆诗";
        //含税法人主体 深圳市艾姆诗数码科技有限公司
        public const string TaxLegalPerson = "深圳市艾姆诗数码科技有限公司";
        //不含税法人主体 英驰先锋科技有限公司
        public const string NonTaxLegalPerson = "英驰先锋科技有限公司";

        /// <summary>
        /// 虚拟仓库
        /// </summary>
        public const string Special_Storage = "艾姆诗仓库";
        /// <summary>
        /// 采购员配置匹配值
        /// </summary>
        public const decimal PurchasermatchValue = 1;

        /// <summary>
        /// 老品处理人
        /// </summary>
        public const string oldProductDeveloper = "老品处理人";

        /// <summary>
        /// 暂无部门负责人
        /// </summary>
        public const string NotDepartmentManager = "暂无部门负责人";

        /// <summary>
        /// 产品列表删除采集图片（原始路径）
        /// </summary>
        public const string OldProductImagePathDelete = "ProductDevelopment";
        //服装类别ID
        public const int ClothingCategoryId = 64;

        /// <summary>
        /// 产品采购问题
        /// </summary>
        public const string PRODUCT_PURCHASE_QUESTION = "ProductPurchaseQuestion";

        /// <summary>
        /// 产品采购问题审核配置
        /// </summary>
        public const string ProductPurchaseQuestionApprovalDateTime = "ProductPurchaseQuestionApprovalDateTime";

        /// <summary>
        /// 赛维分销产品
        /// </summary>
        public const string SupplierCode = "SW1223";

        /// <summary>
        /// 在线文档-公司公告
        /// </summary>
        public const string DocumentsCompanyNotice = "公司公告";
        /// <summary>
        /// 在线文档-重要功能上线通知
        /// </summary>
        public const string DocumentsFunctionNotice = "重要功能上线通知";

        /// <summary>
        /// 公共异常提示信息
        /// </summary>
        public const string errorMessage = "发生未知的错误，请联系管理员！";
        /// <summary>
        /// 公共成功提示信息
        /// </summary>
        public const string successMessage = "操作成功！";

        /// <summary>
        /// 正常产品编码开头用于普通产品搜索
        /// </summary>
        public static readonly string[] NormalProductCodePrefix = new[] { "I0", "I1", "I2", "I3", "I4" };
        /// <summary>
        /// 部门显示-重庆产品开发部
        /// </summary>
        public const string CQDepartmentName = "重庆产品开发部";
        /// <summary>
        /// 部门显示-广州产品开发部
        /// </summary>
        public const string GZDepartmentName = "广州产品开发部";
        /// <summary>
        /// 部门显示-深圳产品开发部
        /// </summary>
        public const string SZDepartmentName = "深圳产品开发部";
        /// <summary>
        /// 部门显示-义乌产品开发部
        /// </summary>
        public const string YWDepartmentName = "义乌产品开发部";
        /// <summary>
        /// 部门显示-服装产品开发组
        /// </summary>
        public const string ClothingDepartmentName = "服装产品开发组";

        /// <summary>
        /// 区域：深圳产品开发部
        /// </summary>
        public const string SZ_AREA = "0";

        /// <summary>
        /// 区域：义务产品开发部
        /// </summary>
        public const string YW_AREA = "5";

        /// <summary>
        /// 区域：重庆产品开发部
        /// </summary>
        public const string CQ_AREA = "6";

        /// <summary>
        /// 区域：广州产品开发部
        /// </summary>
        public const string GZ_AREA = "11";

        /// <summary>
        /// 邮件屏蔽公司id
        /// </summary>
        public const string MailScreeningCompanyId = "directimports1899";

        /// <summary>
        /// 未完成付款单列表-驳回状态  32=待签收驳回,33=待财务审核驳回
        /// </summary>

        public static readonly int[] OpPaymentStatusNewRejects = new[] { 32, 33 };

        /// <summary>
        /// 未完成费用报销单-驳回状态  12=待签收驳回,13=待财务审核驳回
        /// </summary>

        public static readonly int[] NewFeeStockChangeRejects = new[] { 12, 13 };

        /// <summary>
        /// 分析师岗位名称集合
        /// </summary>
        public static readonly string[] ProductAnalysts = new[] { "产品分析师", "高级产品分析师", "中级产品分析师" };

        /// <summary>
        /// 算价分页数量
        /// </summary>
        public const int CalcProductSkuPricePageSize = 1000;

        #region 导师异常消息通知人
        /// <summary>
        /// 艾九钢
        /// </summary>
        public const int TutorTaskMessageToUser = 7132;

        /// <summary>
        /// 尹志远ID
        /// </summary>
        public const int TutorTaskMessageToUserYingzhiYuanId = 9104;
        /// <summary>
        /// 尹志远
        /// </summary>
        public const string TutorTaskMessageToUserYingzhiYuanName = "尹志远";

        /// <summary>
        /// 魏洪芳ID
        /// </summary>
        public const int TutorTaskMessageToUserWeiHongFang = 7193;
        #endregion
        #region 万邑通相关API
        /// <summary>
        /// 万邑通token
        /// </summary>
        public const string IMcWntToken = "89E0AF35C64AA403241E888AE013CA0A";//正式
        /// <summary>
        /// 客户号
        /// </summary>
        public const string IMcWntAppKeyCode = "2355562169@qq.com";//正式

        /// <summary>
        /// client_id
        /// </summary>
        public const string IMCWntClientId = "ZTHHYTLJZJUTZGFHZS00NWM4LWEXZMUTOWFMOGYXMMNLMTFK"; //正式

        /// <summary>
        /// client_secret
        /// </summary>
        public const string IMCWntClientSecret = "YZRHM2M0ZDGTNMY3NY00YTU4LTKYZMYTYJI5NZVIMMYYMJZJMTIXNJIXOTKYNJE2NJC4MJEX"; //正式

        /// <summary>
        /// 版本号
        /// </summary>
        public const string IMcWntVersion = "1.0";//正式
        /// <summary>
        /// URL
        /// </summary>
        public const string IMcWntUrl = "http://openapi.winit.com.cn/openapi/service";//正式

        #endregion

        #region  TrackingMoreAPI相关
        /// <summary>
        /// TrackingMore ApiKey
        /// </summary>
        public const string TrackingMoreApiKey = "f6a30a12-d658-45b9-98b2-07d1c410e577";//正式

        #endregion

        #region 捷网API

        public const string JWclientId = "6585";//clientId
        public const string JWsecretId = "1aa9c10e3f8a4667";//secretId
        public const string JWtoken = "9acd1d7af550a6def0b866f63772d880";//md5加密后的token
        /// <summary>
        /// JW联系人	
        /// </summary>
        //public const string JwContactName = "卢传智";
        //public const string JwContactName = "吴文斌";
        public const string JwContactName = "刘辉琴";
        /// <summary>
        /// 联系电话
        /// </summary>
        //public const string JWContactTel = "13450864304";
        //public const string JWContactTel = "13450275581";
        public const string JWContactTel = "13714843925";



        #endregion

        #region 谷仓api

        #region 沙箱环境

        //public const string IMcGcAppTokenTest = "7013991264f611e98ea200e01b680258";//测试
        //public const string IMcGcAppKeyTest = "6ff50abf64f611e98ea200e01b680258";//测试
        //public const string IMcGcUrlTest = "https://sbx-oms.eminxing.com:60083/default/svc/web-service";//测试

        #endregion

        #region 生产环境

        public const string IMcGcAppToken = "02a79510f186bb92912fd4ddcfc8a980";//谷仓正式
        public const string IMcGcAppKey = "cae09726050fb16dca50c894522ab906";//谷仓正式
        public const string IMcGcUrl = "https://oms.goodcang.com/default/svc/web-service";//谷仓正式

        #endregion

        #region WP西邮海外仓API

        //测试
        //public const string IMCWPAppToken = "32384d19830e9bba0c490839fac0a792";
        //public const string IMCWPAppKey = "63da413a3ad7c0c1161f741e488bf175";
        //public const string IMCWPUrl = "http://47.112.148.108:8082/default/svc/web-service";

        //正式
        public const string IMCWPAppToken = "51958f9fa40685b3a32f723d4ea85cde";
        public const string IMCWPAppKey = "edd716d0f00574fd83903feb705d290f";
        public const string IMCWPUrl = "http://oms.shippingself.com/default/svc/web-service";

        #endregion

        #endregion

        #region 义达api
        //public const string IMcYdAppToken = "7013991264f611e98ea200e01b680258";//测试
        //public const string IMcYdAppKey = "6ff50abf64f611e98ea200e01b680258";//测试
        //public const string IMcYdUrl = "https://sbx-oms.eminxing.com:60083/default/svc/web-service";//测试

        public const string IMcYdhAppToken = "086ffadfa855ed20ce7abfe64d0836e9";//义达正式
        public const string IMcYdhAppKey = "8efece8df430b21a1ca78c18c7539a09";//义达正式
        public const string IMcYdhUrl = "http://47.52.17.241:93/default/svc/web-service";//义达正式

        #endregion

        #region 西邮寄
        public const string IMcWpApiUserId = "fccfb";//账户的会员编号
        public const string IMcWpApiPwd = "I123456";//clientId
        public const string IMcWpApiKey = "722fa67c3ad5629975a0bb006c11b7f8";//西邮寄key
        public const string IMcWpUrl = "http://www.xipost.com.cn/api15.php";//西邮寄正式
        #endregion

        #region 获取客服端使用http数据传输方法
        /// <summary>
        /// 获取客服端使用http数据传输方法字符串GET
        /// </summary>
        public const string HttpMethodGetStr = "GET";
        /// <summary>
        /// 获取客服端使用http数据传输方法字符串POST
        /// </summary>
        public const string HttpMethodPostStr = "POST";
        /// <summary>
        /// 获取客服端使用http数据传输方法字符串HEAD
        /// </summary>
        public const string HttpMethodHeadStr = "HEAD";
        #endregion
        #region 客服
        public const string ProblemTypeEBayNoAscanDescription = "NO-ASCAN订单已退款，请通知客人";//问题类型为16的默认问题描述
        #endregion

        #region 办公资产

        #region 仓库

        // update by pengyoulin 2020-6-27 
        //业务逻辑中 每个区域 都会去指定一个特定的仓库
        //在使用过程中，每个仓库的名称有可能会有变更 这里直接把现有每个仓库的ID存储下来，避免以后经常改动代码

        /// <summary>
        /// 重庆仓库
        /// </summary>
        public static readonly int CQStorageId = 25;
        /// <summary>
        /// 义乌仓库
        /// </summary>
        public static readonly int YWStorageId = 26;
        /// <summary>
        /// 清湖总公司固定资产仓
        /// </summary>
        public static readonly int QHFixedAssetsStorageId = 7;
        /// <summary>
        /// 清湖总公司低值易耗仓
        /// </summary>
        public static readonly int QHLowValueStorageId = 24;
        /// <summary>
        /// 惠州分部
        /// </summary>
        public static readonly int HZStorageId = 9;
        /// <summary>
        /// 南昌
        /// </summary>
        public static readonly int NCStorageId = 12;
        /// <summary>
        /// 宝安分部物流货仓
        /// </summary>
        public static readonly int BAStorageId = 20;
        /// <summary>
        /// 广州分部市场采购仓
        /// </summary>
        public static readonly int GZStorageId = 22;
        /// <summary>
        /// 小包仓(惠州仓库[1],福永仓库[97],惠州B仓库[279],惠州B布料仓[287])
        /// </summary>
        public static readonly int[] SmallStorageIds = { 1, 97, 279, 287 };
        #endregion

        #region 资产类型
        /// <summary>
        /// 固定资产
        /// </summary>
        public static readonly string FixedAssetAssetTypeName = "固定资产";
        /// <summary>
        /// 样品
        /// </summary>
        public static readonly string SampleTypeName = "样品";
        /// <summary>
        /// 低值易耗
        /// </summary>
        public static readonly string LowValueTypeName = "低值易耗";
        /// <summary>
        /// 长期待摊
        /// </summary>
        public static readonly string LongTermDeferrdTypeName = "长期待摊";
        #endregion



        #endregion

        #region 阿里回复信息配置
        /// <summary>
        /// 阿里回复信息配置
        /// </summary>
        public static readonly string AliMessageConfig = "AliMessageConfig";
        /// <summary>
        /// 阿里回复信息配置类型
        /// </summary>
        public static readonly string PriceChange = "改价信息";
        /// <summary>
        /// 阿里回复信息配置类型
        /// </summary>
        public static readonly string DeliverGoods = "发货信息";
        #endregion

        #region 合同确认类型

        /// <summary>
        /// 合同确认类型-采购交接
        /// </summary>
        public static readonly string PurchaseHandOver = "PURCHASE_HANDOVER";
        /// <summary>
        /// 合同确认类型-财务确认
        /// </summary>
        public static readonly string FinanceConfirm = "FINANCE_CONFIRM";
        /// <summary>
        /// 合同确认类型-法务确认
        /// </summary>
        public static readonly string LegalConfirm = "LEGAL_CONFIRM";

        #endregion
    }
}
