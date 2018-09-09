using System;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Vhdx;
using Xunit;

namespace LibraryTests.Vhdx
{
    public class DiskImageFileTest
    {
        [Fact]
        public void InitializeDynamic_When_Physical_Sector_Size_And_Logical_Sector_Size_Are_Default()
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB))
            {
                Assert.NotNull(diskImageFile);
                Assert.Equal(4096, diskImageFile.PhysicalSectorSize);
                Assert.Equal(512, diskImageFile.LogicalSectorSize);
            }
        }

        [Theory]
        [InlineData(512, 512)] // 512-N disk
        [InlineData(4096, 512)] // 512-E disk (aka "Advanced Format")
        [InlineData(4096, 4096)] // 4K-N disk
        public void InitializeDynamic_With_User_Defined_Physical_And_Logical_Sector_Sizes(int physicalSectorSize, int logicalSectorSize)
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * Sizes.OneMiB,
                physicalSectorSize: physicalSectorSize, logicalSectorSize: logicalSectorSize))
            {
                Assert.NotNull(diskImageFile);
                Assert.Equal(physicalSectorSize, diskImageFile.PhysicalSectorSize);
                Assert.Equal(logicalSectorSize, diskImageFile.LogicalSectorSize);
            }
        }

        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        public void InitializeDynamic_With_Invalid_Phyiscal_Sector_Size(int physicalSectorSize)
        {
            using (var ms = new MemoryStream())
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * Sizes.OneMiB,
                        physicalSectorSize: physicalSectorSize, logicalSectorSize: 4096));
                Assert.Equal("physicalSectorSize", exception.ParamName);
            }
        }

        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        public void InitializeDynamic_With_Invalid_Logical_Sector_Size(int logicalSectorSize)
        {
            using (var ms = new MemoryStream())
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * Sizes.OneMiB,
                        physicalSectorSize: 4096, logicalSectorSize: logicalSectorSize));
                Assert.Equal("logicalSectorSize", exception.ParamName);
            }
        }
    }
}