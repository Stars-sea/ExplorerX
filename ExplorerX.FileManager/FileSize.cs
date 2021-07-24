using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ExplorerX.FileManager {
	public record SizeUnit(string Alias, SizeUnit? BaseUnit, uint Rate = 1024, [CallerMemberName] string Name = "") {
		#region Units
		public static readonly SizeUnit Bit			= new("b", null, 1);
		public static readonly SizeUnit Byte		= new("B", Bit, 8);
		public static readonly SizeUnit Kilobyte	= new("KB", Byte);
		public static readonly SizeUnit Megabyte    = new("MB", Kilobyte);
		public static readonly SizeUnit Gigabyte    = new("GB", Megabyte);
		public static readonly SizeUnit Terabyte    = new("TB", Gigabyte);
		public static readonly SizeUnit Petabyte    = new("PB", Terabyte);
		public static readonly SizeUnit Exabyte     = new("EB", Petabyte);
		public static readonly SizeUnit Zettabyte   = new("ZB", Exabyte);
		public static readonly SizeUnit Yottabyte   = new("YB", Zettabyte);
		public static readonly SizeUnit Brontobyte  = new("BB", Yottabyte);
		#endregion

		private static readonly SizeUnit[] Units = {
			Bit, Byte, Kilobyte, Megabyte, Gigabyte, Terabyte, Petabyte, Exabyte, Zettabyte, Yottabyte, Brontobyte
		};

		private SizeUnit? largerUnit;

		/// <summary>
		/// Get the larger unit. If don't have, it will return itself.
		/// </summary>
		public  SizeUnit  LargerUnit => largerUnit ??= GetLargerUnit();

		private SizeUnit GetLargerUnit()
			=> Units.SingleOrDefault(unit => unit.BaseUnit == this) ?? this;
	}

	public record FileSize(BigInteger Size) {
		public static FileSize operator +(FileSize left, FileSize right)
			=> new(left.Size + right.Size);

		public static FileSize operator -(FileSize left, FileSize right)
			=> new(left.Size - right.Size);

		public static explicit operator BigInteger(FileSize size) => size.Size;

		public override string ToString() {
			// 因为 BigInteger 只能处理整数, 所以给它"加"了小数点后三位
			const uint rate = 1000;
			BigInteger decimalSize = Size * rate;

			SizeUnit currentUnit = SizeUnit.Byte;
			while (decimalSize >= 10 * rate && currentUnit != currentUnit.LargerUnit) {
				decimalSize /= (currentUnit = currentUnit.LargerUnit).Rate;
			}

			double realResult = ((double) decimalSize) / rate;
			return $"{Math.Round(realResult, 2)} {currentUnit.Alias}";
		}
	}
}
