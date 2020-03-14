using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class StorageUnit
    {
        public static SizeUnit AutoConver(long size)
        {
            SizeUnit sizeunit = new SizeUnit();

            if (size / 1099511627776.0 > 1)      // TB
            {
                sizeunit.size = Math.Round(size / 1099511627776.0, 2);
                sizeunit.unit = "TB";
            }
            else if (size / 1073741824.0 > 1)           // GB
            {
                sizeunit.size = Math.Round(size / 1073741824.0, 2);
                sizeunit.unit = "GB";
            }
            else if (size / 1048576.0 > 1)           // MB
            {
                sizeunit.size = Math.Round(size / 1048576.0, 2);
                sizeunit.unit = "MB";
            }
            else if (size / 1024.0 > 1)
            {
                sizeunit.size = Math.Round(size / 1024.0, 2);
                sizeunit.unit = "KB";
            }
            else
            {
                sizeunit.size = size;
                sizeunit.unit = "B";
            }

            return sizeunit;
        }
    }

    public class SizeUnit
    {
        public double size { get; set; }
        public string unit { get; set; }
    }
}
