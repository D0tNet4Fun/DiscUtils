using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiscUtils.Streams;
using DiscUtils.Vhdx;
using Xunit;

namespace LibraryTests.Vhdx
{
    public class DiskImageFileTest
    {
        [Fact]
        public void InitializeFixed_With_Default_Parameters()
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeFixed(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB))
            {
                Assert.NotNull(diskImageFile);
                Assert.False(diskImageFile.IsSparse);
                Assert.Equal(4096, diskImageFile.PhysicalSectorSize);
                Assert.Equal(512, diskImageFile.LogicalSectorSize);
                Assert.Equal((4 + 128) * Sizes.OneMiB, ms.Length); // the stream length is 4 MB header + 128 MB
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(16)]
        [InlineData(32)]
        public void InitializeFixed_With_User_Defined_Block_Size(int blockSize)
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeFixed(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: blockSize * (int)Sizes.OneMiB,
                physicalSectorSize: 4096, logicalSectorSize: 512))
            {
                Assert.NotNull(diskImageFile);
                Assert.False(diskImageFile.IsSparse);
            }
        }

        [Theory]
        [InlineData(512, 512)] // 512-N disk
        [InlineData(4096, 512)] // 512-E disk (aka "Advanced Format")
        [InlineData(4096, 4096)] // 4K-N disk
        public void InitializeFixed_With_User_Defined_Sector_Sizes(int physicalSectorSize, int logicalSectorSize)
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeFixed(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * (int)Sizes.OneMiB,
                physicalSectorSize: physicalSectorSize, logicalSectorSize: logicalSectorSize))
            {
                Assert.NotNull(diskImageFile);
                Assert.False(diskImageFile.IsSparse);
                Assert.Equal(physicalSectorSize, diskImageFile.PhysicalSectorSize);
                Assert.Equal(logicalSectorSize, diskImageFile.LogicalSectorSize);
                Assert.Equal((4 + 128) * Sizes.OneMiB, ms.Length); // the stream length is 4 MB header + 128 MB
            }
        }

        [Fact]
        public void InitializeFixed_With_Progress_Reporting()
        {
            var capacity = 128 * Sizes.OneMiB;

            var progressValues = new List<long>();
            IProgress<long> bytesWrittenProgress = new Progress<long>(value => { progressValues.Add(value); });

            using (var ms = new MemoryStream())
            using (DiskImageFile.InitializeFixed(ms, Ownership.Dispose, capacity: capacity, blockSize: 32 * (int)Sizes.OneMiB,
                physicalSectorSize: 4096, logicalSectorSize: 512, progress: bytesWrittenProgress))
            {
                // ignore it
            }

            Assert.Equal(0, progressValues.First());
            Assert.Equal(capacity, progressValues.Last());
        }

        [Fact]
        public void InitializeDynamic_With_Default_Parameters()
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB))
            {
                Assert.NotNull(diskImageFile);
                Assert.True(diskImageFile.IsSparse);
                Assert.Equal(4096, diskImageFile.PhysicalSectorSize);
                Assert.Equal(512, diskImageFile.LogicalSectorSize);
            }
        }

        [Theory]
        [InlineData(512, 512)] // 512-N disk
        [InlineData(4096, 512)] // 512-E disk (aka "Advanced Format")
        [InlineData(4096, 4096)] // 4K-N disk
        public void InitializeDynamic_With_User_Defined_Sector_Sizes(int physicalSectorSize, int logicalSectorSize)
        {
            using (var ms = new MemoryStream())
            using (var diskImageFile = DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * (int)Sizes.OneMiB,
                physicalSectorSize: physicalSectorSize, logicalSectorSize: logicalSectorSize))
            {
                Assert.NotNull(diskImageFile);
                Assert.True(diskImageFile.IsSparse);
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
                    DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * (int)Sizes.OneMiB,
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
                    DiskImageFile.InitializeDynamic(ms, Ownership.Dispose, capacity: 128 * Sizes.OneMiB, blockSize: 32 * (int)Sizes.OneMiB,
                        physicalSectorSize: 4096, logicalSectorSize: logicalSectorSize));
                Assert.Equal("logicalSectorSize", exception.ParamName);
            }
        }
    }
}