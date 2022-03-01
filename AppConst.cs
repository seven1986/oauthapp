using Microsoft.Extensions.PlatformAbstractions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OAuthApp
{
    public static class AppConst
    {
        public static readonly string BlobServer = "https://blob.oauthapp.com";

        public static readonly string AssemblyName = Assembly.GetEntryAssembly().GetName().Name;

        public static readonly string TenantDBPath = Path.Combine(
            PlatformServices.Default.Application.ApplicationBasePath,
            "tenant_db");

        public static readonly string TenantDBConnection = "Data Source=" + Path.Combine(TenantDBPath, "oauthapp.db");
    }


    public class ChannelCodes
    {
        public const string Project = "project";
        public const string App = "app";
        public const string Api = "api";
        public const string CodeGen = "codegen";

        public const string Tenant = "tenant";
        public const string AppBlob = "appblob";
        public const string AppVersion = "appversion";
    }

    public class PropertyTag
    {
        public const string Server = "server";
        public const string Client = "client";
    }

    public class UserConst
    {
        public const string DefaultPlatform = "_";
        public const long DefaultAppID = 1;
        public const string DefaultAppUserRole = "user";
    }

    public class PropKeyConst
    {
        public const string OpenWechatClientID = "OpenWechatClientID";
        public const string OpenWechatClientSecret = "OpenWechatClientSecret";
        public const string OpenWechatScope = "OpenWechatScope";
        public const string OpenWechatRedirectUri = "OpenWechatRedirectUri";

        public const string WechatClientID = "WechatClientID";
        public const string WechatClientSecret = "WechatClientSecret";
        public const string WechatScope = "WechatScope";
        public const string WechatRedirectUri = "WechatRedirectUri";

        public const string WechatMiniPClientID = "WechatMiniPClientID";
        public const string WechatMiniPClientSecret = "WechatMiniPClientSecret";

        public const string QQClientID = "QQClientID";
        public const string QQClientSecret = "QQClientSecret";
        public const string QQRedirectUri = "QQRedirectUri";

        public const string WeiboClientID = "WeiboClientID";
        public const string WeiboClientSecret = "WeiboClientSecret";
        public const string WeiboRedirectUri = "WeiboRedirectUri";

        public const string WechatJSApiList = "WechatJSApiList";
    }

    public class DataSeed
    {
        public const string AppKey = "bf392318-af92-4056-bf51-bf51cdf8f20e";
    }

    public class ContentTypes
    {
        public static Dictionary<string, string> Types = new Dictionary<string, string>()
        {
            {".ez","application/andrew-inset"},
            {".hqx","application/mac-binhex40"},
            {".cpt","application/mac-compactpro"},
            {".doc","application/msword"},
            {".bin","application/octet-stream"},
            {".dms","application/octet-stream"},
            {".lha","application/octet-stream"},
            {".lzh","application/octet-stream"},
            {".exe","application/octet-stream"},
            {".class","application/octet-stream"},
            {".so","application/octet-stream"},
            {".dll","application/octet-stream"},
            {".oda","application/oda"},
            {".pdf","application/pdf"},
            {".ai","application/postscript"},
            {".eps","application/postscript"},
            {".ps","application/postscript"},
            {".smi","application/smil"},
            {".smil","application/smil"},
            {".mif","application/vnd.mif"},
            {".xls","application/vnd.ms-excel"},
            {".ppt","application/vnd.ms-powerpoint"},
            {".wbxml","application/vnd.wap.wbxml"},
            {".wmlc","application/vnd.wap.wmlc"},
            {".wmlsc","application/vnd.wap.wmlscriptc"},
            {".bcpio","application/x-bcpio"},
            {".vcd","application/x-cdlink"},
            {".pgn","application/x-chess-pgn"},
            {".cpio","application/x-cpio"},
            {".csh","application/x-csh"},
            {".dcr","application/x-director"},
            {".dir","application/x-director"},
            {".dxr","application/x-director"},
            {".dvi","application/x-dvi"},
            {".spl","application/x-futuresplash"},
            {".gtar","application/x-gtar"},
            {".hdf","application/x-hdf"},
            {".js","application/x-javascript"},
            {".skp","application/x-koan"},
            {".skd","application/x-koan"},
            {".skt","application/x-koan"},
            {".skm","application/x-koan"},
            {".latex","application/x-latex"},
            {".nc","application/x-netcdf"},
            {".cdf","application/x-netcdf"},
            {".sh","application/x-sh"},
            {".shar","application/x-shar"},
            {".swf","application/x-shockwave-flash"},
            {".sit","application/x-stuffit"},
            {".sv4cpio","application/x-sv4cpio"},
            {".sv4crc","application/x-sv4crc"},
            {".tar","application/x-tar"},
            {".tcl","application/x-tcl"},
            {".tex","application/x-tex"},
            {".texinfo","application/x-texinfo"},
            {".texi","application/x-texinfo"},
            {".tr","application/x-troff"},
            {".roff","application/x-troff"},
            {".man","application/x-troff-man"},
            {".me","application/x-troff-me"},
            {".ms","application/x-troff-ms"},
            {".ustar","application/x-ustar"},
            {".src","application/x-wais-source"},
            {".xhtml","application/xhtml+xml"},
            {".xht","application/xhtml+xml"},
            {".zip","application/zip"},
            {".au","audio/basic"},
            {".snd","audio/basic"},
            {".mid","audio/midi"},
            {".midi","audio/midi"},
            {".kar","audio/midi"},
            {".mpga","audio/mpeg"},
            {".mp2","audio/mpeg"},
            {".mp3","audio/mpeg"},
            {".aif","audio/x-aiff"},
            {".aiff","audio/x-aiff"},
            {".aifc","audio/x-aiff"},
            {".m3u","audio/x-mpegurl"},
            {".ram","audio/x-pn-realaudio"},
            {".rm","audio/x-pn-realaudio"},
            {".rpm","audio/x-pn-realaudio-plugin"},
            {".ra","audio/x-realaudio"},
            {".wav","audio/x-wav"},
            {".pdb","chemical/x-pdb"},
            {".xyz","chemical/x-xyz"},
            {".bmp","image/bmp"},
            {".gif","image/gif"},
            {".ief","image/ief"},
            {".jpeg","image/jpeg"},
            {".jpg","image/jpeg"},
            {".jpe","image/jpeg"},
            {".png","image/png"},
            {".tiff","image/tiff"},
            {".tif","image/tiff"},
            {".djvu","image/vnd.djvu"},
            {".djv","image/vnd.djvu"},
            {".wbmp","image/vnd.wap.wbmp"},
            {".ras","image/x-cmu-raster"},
            {".pnm","image/x-portable-anymap"},
            {".pbm","image/x-portable-bitmap"},
            {".pgm","image/x-portable-graymap"},
            {".ppm","image/x-portable-pixmap"},
            {".rgb","image/x-rgb"},
            {".xbm","image/x-xbitmap"},
            {".xpm","image/x-xpixmap"},
            {".xwd","image/x-xwindowdump"},
            {".igs","model/iges"},
            {".iges","model/iges"},
            {".msh","model/mesh"},
            {".mesh","model/mesh"},
            {".silo","model/mesh"},
            {".wrl","model/vrml"},
            {".vrml","model/vrml"},
            {".css","text/css"},
            {".html","text/html"},
            {".htm","text/html"},
            {".asc","text/plain"},
            {".txt","text/plain"},
            {".rtx","text/richtext"},
            {".rtf","text/rtf"},
            {".sgml","text/sgml"},
            {".sgm","text/sgml"},
            {".tsv","text/tab-separated-values"},
            {".wml","text/vnd.wap.wml"},
            {".wmls","text/vnd.wap.wmlscript"},
            {".etx","text/x-setext"},
            {".xsl","text/xml"},
            {".xml","text/xml"},
            {".mpeg","video/mpeg"},
            {".mpg","video/mpeg"},
            {".mpe","video/mpeg"},
            {".qt","video/quicktime"},
            {".mov","video/quicktime"},
            {".mxu","video/vnd.mpegurl"},
            {".avi","video/x-msvideo"},
            {".movie","video/x-sgi-movie"},
            {".ice","x-conference/x-cooltalk"}
        };
    }
}
