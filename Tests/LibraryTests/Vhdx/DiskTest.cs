using System;
using System.IO;
using System.Reflection;
using DiscUtils;
using DiscUtils.Setup;
using DiscUtils.Streams;
using DiscUtils.Vhdx;
using Xunit;

namespace LibraryTests.Vhdx
{
    public class DiskTest
    {
        [Fact]
        public void CreateDisk_Using_Parameters()
        {
            SetupHelper.RegisterAssembly(typeof(Disk).GetTypeInfo().Assembly);
            var diskParameters = new VirtualDiskParameters
            {
                Capacity = 32 * Sizes.OneMiB,
                ExtendedParameters =
                {
                    { "VHDX.BlockSize", (32 * Sizes.OneMiB).ToString() },
                    { "VHDX.PhysicalSectorSize", (4 * Sizes.OneKiB).ToString() },
                    { "VHDX.LogicalSectorSize", (4 * Sizes.OneKiB).ToString() }
                }
            };

            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.vhdx");
            try
            {
                using (var disk = VirtualDisk.CreateDisk("VHDX", "dynamic", path, diskParameters, string.Empty, string.Empty))
                {
                    var parameters = disk.Parameters;
                    Assert.Equal((32 * Sizes.OneMiB).ToString(), parameters.ExtendedParameters["VHDX.BlockSize"]);
                    Assert.Equal((4 * Sizes.OneKiB).ToString(), parameters.ExtendedParameters["VHDX.PhysicalSectorSize"]);
                    Assert.Equal((4 * Sizes.OneKiB).ToString(), parameters.ExtendedParameters["VHDX.LogicalSectorSize"]);
                }
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}