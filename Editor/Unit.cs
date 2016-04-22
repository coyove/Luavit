using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	internal static class Unit
	{
		public static double CovertFromAPI(DisplayUnitType to, double value)
		{
			switch (to)
			{
			case (DisplayUnitType)56:
				return value * Unit.ImperialDutRatio(to) - 459.67;
			case (DisplayUnitType)57:
				return value - 273.15;
			default:
				return value *= Unit.ImperialDutRatio(to);
			}
		}

		public static double CovertToAPI(double value, DisplayUnitType from)
		{
			switch (from)
			{
			case (DisplayUnitType)56:
				return (value + 459.67) / Unit.ImperialDutRatio(from);
			case (DisplayUnitType)57:
				return value + 273.15;
			default:
				return value /= Unit.ImperialDutRatio(from);
			}
		}

		private static double ImperialDutRatio(DisplayUnitType dut)
		{
			switch (dut)
			{
			case (DisplayUnitType)0:
				return 0.3048;
			case (DisplayUnitType)1:
				return 30.48;
			case (DisplayUnitType)2:
				return 304.8;
			case (DisplayUnitType)3:
				return 1.0;
			case (DisplayUnitType)4:
				return 1.0;
			case (DisplayUnitType)5:
				return 12.0;
			case (DisplayUnitType)6:
				return 12.0;
			case (DisplayUnitType)7:
				return 2.29568411386593E-05;
			case (DisplayUnitType)8:
				return 9.290304E-06;
			case (DisplayUnitType)9:
				return 0.3048;
			case (DisplayUnitType)10:
				return 0.037037037037037;
			case (DisplayUnitType)11:
				return 1.0;
			case (DisplayUnitType)12:
				return 0.09290304;
			case (DisplayUnitType)13:
				return 1.0;
			case (DisplayUnitType)14:
				return 0.028316846592;
			case (DisplayUnitType)15:
				return 57.2957795130823;
			case (DisplayUnitType)16:
				return 57.2957795130823;
			case (DisplayUnitType)17:
				return 1.0;
			case (DisplayUnitType)18:
				return 1.0;
			case (DisplayUnitType)19:
				return 100.0;
			case (DisplayUnitType)20:
				return 144.0;
			case (DisplayUnitType)21:
				return 929.0304;
			case (DisplayUnitType)22:
				return 92903.04;
			case (DisplayUnitType)23:
				return 1728.0;
			case (DisplayUnitType)24:
				return 28316.846592;
			case (DisplayUnitType)25:
				return 28316846.592;
			case (DisplayUnitType)26:
				return 28.316846592;
			case (DisplayUnitType)27:
				return 7.48051905367236;
			case (DisplayUnitType)28:
				return 35.3146667214886;
			case (DisplayUnitType)29:
				return 2.20462262184878;
			case (DisplayUnitType)30:
				return 0.00127582327653286;
			case (DisplayUnitType)31:
				return 8.80550918411529E-05;
			case (DisplayUnitType)32:
				return 0.0221895098882201;
			case (DisplayUnitType)33:
				return 2.21895098882201E-05;
			case (DisplayUnitType)34:
				return 0.09290304;
			case (DisplayUnitType)35:
				return 2.58064E-08;
			case (DisplayUnitType)36:
				return 8.80547457016663E-10;
			case (DisplayUnitType)37:
				return 1.31845358262865;
			case (DisplayUnitType)38:
				return 10.7639104167097;
			case (DisplayUnitType)39:
				return 0.09290304;
			case (DisplayUnitType)40:
				return 9.290304E-05;
			case (DisplayUnitType)41:
				return 8.80550918411529E-05;
			case (DisplayUnitType)42:
				return 0.316998330628151;
			case (DisplayUnitType)43:
				return 0.0221895098882201;
			case (DisplayUnitType)44:
				return 2.21895098882201E-05;
			case (DisplayUnitType)45:
				return 0.09290304;
			case (DisplayUnitType)46:
				return 1.0;
			case (DisplayUnitType)47:
				return 0.0131845358262865;
			case (DisplayUnitType)48:
				return 3.28083989501312;
			case (DisplayUnitType)49:
				return 0.00328083989501312;
			case (DisplayUnitType)50:
				return 3.28083989501312E-06;
			case (DisplayUnitType)51:
				return 0.000475845616460903;
			case (DisplayUnitType)52:
				return 0.000968831370233344;
			case (DisplayUnitType)53:
				return 0.0246083170946002;
			case (DisplayUnitType)54:
				return 3.23793722675857E-05;
			case (DisplayUnitType)55:
				return 3.28083989501312E-05;
			case (DisplayUnitType)56:
				return 1.8;
			case (DisplayUnitType)57:
				return 1.0;
			case (DisplayUnitType)58:
				return 1.0;
			case (DisplayUnitType)59:
				return 1.8;
			case (DisplayUnitType)60:
				return 60.0;
			case (DisplayUnitType)61:
				return 0.3048;
			case (DisplayUnitType)62:
				return 1828.8;
			case (DisplayUnitType)63:
				return 60.0;
			case (DisplayUnitType)64:
				return 28.316846592;
			case (DisplayUnitType)65:
				return 0.028316846592;
			case (DisplayUnitType)66:
				return 101.9406477312;
			case (DisplayUnitType)67:
				return 448.831143220342;
			case (DisplayUnitType)68:
				return 26929.8685932205;
			case (DisplayUnitType)69:
				return 1.0;
			case (DisplayUnitType)70:
				return 0.001;
			case (DisplayUnitType)71:
				return 1000.0;
			case (DisplayUnitType)72:
				return 0.09290304;
			case (DisplayUnitType)73:
				return 9.290304E-05;
			case (DisplayUnitType)74:
				return 92.90304;
			case (DisplayUnitType)75:
				return 1.0;
			case (DisplayUnitType)76:
				return 1.0;
			case (DisplayUnitType)77:
				return 10.7639104167097;
			case (DisplayUnitType)78:
				return 1.0000000387136;
			case (DisplayUnitType)79:
				return 3.1415927449471;
			case (DisplayUnitType)80:
				return 10.7639104167097;
			case (DisplayUnitType)81:
				return 1.0;
			case (DisplayUnitType)82:
				return 1.0;
			case (DisplayUnitType)83:
				return 1.0;
			case (DisplayUnitType)84:
				return 0.09290304;
			case (DisplayUnitType)85:
				return 9.290304E-05;
			case (DisplayUnitType)86:
				return 0.00012458502883053;
			case (DisplayUnitType)87:
				return 0.3048;
			case (DisplayUnitType)88:
				return 0.03048;
			case (DisplayUnitType)89:
				return 0.0003048;
			case (DisplayUnitType)90:
				return 3.048E-07;
			case (DisplayUnitType)91:
				return 6.85217565069691E-05;
			case (DisplayUnitType)92:
				return 0.0310810655372411;
			case (DisplayUnitType)93:
				return 3.10810655372411E-05;
			case (DisplayUnitType)94:
				return 224.80894309971;
			case (DisplayUnitType)95:
				return 1.0;
			case (DisplayUnitType)96:
				return 0.1;
			case (DisplayUnitType)97:
				return 0.001;
			case (DisplayUnitType)98:
				return 1E-06;
			case (DisplayUnitType)99:
				return 6.85217658567918E-05;
			case (DisplayUnitType)100:
				return 0.101971999794098;
			case (DisplayUnitType)101:
				return 0.000101971999794098;
			case (DisplayUnitType)102:
				return 0.0685217658567918;
			case (DisplayUnitType)103:
				return 3.28083989501312;
			case (DisplayUnitType)104:
				return 0.328083989501312;
			case (DisplayUnitType)105:
				return 0.00328083989501312;
			case (DisplayUnitType)106:
				return 3.28083989501312E-06;
			case (DisplayUnitType)107:
				return 6.85217658567918E-05;
			case (DisplayUnitType)108:
				return 0.334553805098747;
			case (DisplayUnitType)109:
				return 0.000334553805098747;
			case (DisplayUnitType)110:
				return 0.0685217658567917;
			case (DisplayUnitType)111:
				return 0.09290304;
			case (DisplayUnitType)112:
				return 0.009290304;
			case (DisplayUnitType)113:
				return 9.290304E-05;
			case (DisplayUnitType)114:
				return 9.290304E-08;
			case (DisplayUnitType)115:
				return 6.85217658567918E-05;
			case (DisplayUnitType)116:
				return 0.00947350877575109;
			case (DisplayUnitType)117:
				return 9.47350877575109E-06;
			case (DisplayUnitType)118:
				return 0.0685217658567918;
			case (DisplayUnitType)119:
				return 1000.0;
			case (DisplayUnitType)120:
				return 14593.9029372064;
			case (DisplayUnitType)121:
				return 304.8;
			case (DisplayUnitType)122:
				return 14593.9029372064;
			case (DisplayUnitType)123:
				return 92.90304;
			case (DisplayUnitType)124:
				return 14593.9029372064;
			case (DisplayUnitType)125:
				return 3280.83989501312;
			case (DisplayUnitType)126:
				return 14593.9029372064;
			case (DisplayUnitType)127:
				return 0.109761336731934;
			case (DisplayUnitType)128:
				return 0.00109764531546318;
			case (DisplayUnitType)129:
				return 3.28083989501312;
			case (DisplayUnitType)130:
				return 2.20462262184878;
			case (DisplayUnitType)131:
				return 3280.83989501312;
			case (DisplayUnitType)132:
				return 1.0;
			case (DisplayUnitType)133:
				return 4.75845596227721E-07;
			case (DisplayUnitType)134:
				return 0.0107639104167097;
			case (DisplayUnitType)135:
				return 0.0685217658567918;
			case (DisplayUnitType)136:
				return 3.96537996856434E-08;
			case (DisplayUnitType)137:
				return 0.555555555555556;
			case (DisplayUnitType)138:
				return 1.0;
			case (DisplayUnitType)139:
				return 0.3048;
			case (DisplayUnitType)140:
				return 0.03048;
			case (DisplayUnitType)141:
				return 0.0003048;
			case (DisplayUnitType)142:
				return 3.048E-07;
			case (DisplayUnitType)143:
				return 6.85217658567918E-05;
			case (DisplayUnitType)144:
				return 0.0310810655372411;
			case (DisplayUnitType)145:
				return 3.10810655372411E-05;
			case (DisplayUnitType)146:
				return 0.0685217658567918;
			case (DisplayUnitType)147:
				return 7936.64143865559;
			case (DisplayUnitType)148:
				return 5.71014715473265E-06;
			case (DisplayUnitType)149:
				return 6.85217658567918E-05;
			case (DisplayUnitType)150:
				return 6.85217658567918E-05;
			case (DisplayUnitType)151:
				return 9.290304E-05;
			case (DisplayUnitType)152:
				return 2.08854342331501E-05;
			case (DisplayUnitType)153:
				return 0.0003048;
			case (DisplayUnitType)154:
				return 1.0;
			case (DisplayUnitType)155:
				return 0.176110194261872;
			case (DisplayUnitType)156:
				return 60.0;
			case (DisplayUnitType)157:
				return 304.8;
			case (DisplayUnitType)158:
				return 10.0;
			case (DisplayUnitType)159:
				return 12.0;
			case (DisplayUnitType)160:
				return 57.2957795130824;
			case (DisplayUnitType)161:
				return 12.0;
			case (DisplayUnitType)162:
				return 1.0;
			case (DisplayUnitType)163:
				return 1000.0;
			case (DisplayUnitType)164:
				return 0.09290304;
			case (DisplayUnitType)165:
				return 3.28083989501312;
			case (DisplayUnitType)166:
				return 0.316998330628151;
			case (DisplayUnitType)167:
				return 0.31699833062815;
			case (DisplayUnitType)168:
				return 2.64165275523459E-05;
			case (DisplayUnitType)169:
				return 60.0;
			case (DisplayUnitType)170:
				return 1000.0;
			case (DisplayUnitType)171:
				return 2271305.33644539;
			case (DisplayUnitType)172:
				return 304800.0;
			case (DisplayUnitType)173:
				return 37855.0889407566;
			case (DisplayUnitType)174:
				return 1000.0;
			case (DisplayUnitType)175:
				return 1.0;
			case (DisplayUnitType)176:
				return 10.7639104167097;
			default:
				return 1.0;
			}
		}

		public static bool DoubleEquals(double left, double right)
		{
			return System.Math.Abs(left - right) < 1E-07;
		}
	}
}
