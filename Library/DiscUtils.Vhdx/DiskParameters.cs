using System;

namespace DiscUtils.Vhdx
{
    /// <summary>
    /// The parameters used to create a new VHDX file.
    /// </summary>
    public sealed class DiskParameters
    {
        private int _blockSize;
        private int _physicalSectorSize;
        private int _logicalSectorSize;

        /// <summary>
        /// Initializes a new instance of the DiskParameters class with default values.
        /// </summary>
        public DiskParameters()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DiskParameters class with generic parameters.
        /// </summary>
        /// <param name="genericParameters">The generic parameters to copy.</param>
        public DiskParameters(VirtualDiskParameters genericParameters)
        {
            Capacity = genericParameters.Capacity;

            string stringBlockSize;
            if (genericParameters.ExtendedParameters.TryGetValue(Disk.ExtendedParameterKeyBlockSize, out stringBlockSize))
            {
                Int32.TryParse(stringBlockSize, out _blockSize);
            }

            string stringPhysicalSectorSize;
            if (genericParameters.ExtendedParameters.TryGetValue(Disk.ExtendedParameterKeyPhysicalSectorSize, out stringPhysicalSectorSize))
            {
                Int32.TryParse(stringPhysicalSectorSize, out _physicalSectorSize);
            }

            string stringLogicalSectorSize;
            if (genericParameters.ExtendedParameters.TryGetValue(Disk.ExtendedParameterKeyLogicalSectorSize, out stringLogicalSectorSize))
            {
                Int32.TryParse(stringLogicalSectorSize, out _logicalSectorSize);
            }
        }

        /// <summary>
        /// Gets or sets the capacity of the virtual disk.
        /// </summary>
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the block size of the virtual disk.
        /// </summary>
        public int BlockSize
        {
            get { return _blockSize != 0 ? _blockSize : (int)FileParameters.DefaultBlockSize; }
            set { _blockSize = value; }
        }

        /// <summary>
        /// Gets or sets the physical sector size of the virtual disk.
        /// </summary>
        public int PhysicalSectorSize
        {
            get { return _physicalSectorSize != 0 ? _physicalSectorSize : (int)FileParameters.DefaultPhysicalSectorSize; }
            set { _physicalSectorSize = value; }
        }

        /// <summary>
        /// Gets or sets the logical sector size of the virtual disk.
        /// </summary>
        public int LogicalSectorSize
        {
            get { return _logicalSectorSize != 0 ? _logicalSectorSize : (int)FileParameters.DefaultLogicalSectorSize; }
            set { _logicalSectorSize = value; }
        }
    }
}
