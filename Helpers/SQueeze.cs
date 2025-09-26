using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// Huffman compression class
    /// </summary>
    /// <remarks>Ported from a D3/mvBase Pick databasic application</remarks>
    public class SQueeZe
    {
        private const int iMinLen = 75; // Minimum length before shortening Scan.Item
        private const int BYTESIZE = 8;
        private const string sMask = "00000000"; // 8-char mask for binary padding

        private bool bBuiltTable = false;
        private int iDecodePointer = 1;
        private int iDecodeCounter = 1;
        private const string sChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+-={}[]:|'\\<>?~,./1234567890 ";
        private string[] sPrimes = new string[503]; // Array to hold prime numbers as strings
        private const int iMod = 239;

        private static readonly string[] asBinTable = new[]
        {
            "0000", "0001", "0010", "0011",    "0100", "0101", "0110", "0111",    "1000", "1001", "1010", "1011",    "1100", "1101", "1110", "1111"
        };

        public SQueeZe()
        {
            if (!bBuiltTable)
            {
                bBuiltTable = true;
                // Initialize the prime string                
                sPrimes = BuildPrimeString(503).Split(',');
            }
        }

        static string BuildPrimeString(int limit)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 2; i <= limit; i++)
            {
                if (IsPrime(i))
                {
                    sb.Append(i);
                    sb.Append(",");
                }
            }

            return sb.ToString().Trim();
        }
        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            int sqrt = (int)Math.Sqrt(number);
            for (int i = 3; i <= sqrt; i += 2)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Compress a string using a custom algorithm, returning a compressed string.
        /// </summary>
        /// <param name="stringToCompress">String to compress</param>
        /// <param name="chkSum">chkSum result</param>
        /// <param name="iMinLen">You can adjust this as needed</param>
        /// <returns></returns>
        public string SQueeZeIT(string stringToCompress, out long chkSum, int iMinLen = 1024)
        {
            const int BYTESIZE = 8;
            int iBitSize = 2;
            int lWorkingSize = 4;
            int iPointer = 2;
            long lCounter = 1;
            string sWorkString = "";
            string sBuffer = "";
            string sWTable = "";
            string sWorkItem = stringToCompress.Length > 0 ? stringToCompress.Substring(0, 1) : "";
            long lCurrentWorkSize = 1;
            long lOldPosInTable = 0;
            long lOldLenWorkItem = 0;
            bool bCompleted = false;
            chkSum = 0;
            if (sWorkItem.Length > 0)
                chkSum += sWorkItem[0];

            int lLenStringToCompress = stringToCompress.Length + 1;
            string result = "";

            while (!bCompleted)
            {
                string sThisChar = (iPointer - 1 < stringToCompress.Length) ? stringToCompress.Substring(iPointer - 1, 1) : "";
                if (string.IsNullOrEmpty(sThisChar))
                {
                    // Input exhausted
                    sWorkString += GetBinaryString(sWorkItem, iBitSize, lOldPosInTable);
                    bCompleted = true;
                }
                else
                {
                    chkSum += sThisChar[0];
                    string sTemp = sWorkItem + sThisChar;
                    int lPosInTable = sWTable.IndexOf(sTemp, StringComparison.Ordinal) + 1;
                    if (lPosInTable > 0)
                    {
                        sWorkItem = sTemp;
                        lOldPosInTable = lPosInTable;
                        lOldLenWorkItem = sTemp.Length;
                    }
                    else
                    {
                        sWorkString += GetBinaryString(sWorkItem, iBitSize, lOldPosInTable);
                        sWTable += sWorkItem;
                        lCurrentWorkSize += sWorkItem.Length;
                        sWorkItem = sThisChar;
                        if (lCurrentWorkSize >= lWorkingSize)
                        {
                            lWorkingSize *= 2;
                            iBitSize += 1;
                        }
                    }
                }

                if (lCounter > lLenStringToCompress)
                    bCompleted = true;

                if (iPointer > iMinLen)
                {
                    stringToCompress = stringToCompress.Substring(iPointer);
                    iPointer = 1;
                }
                else
                {
                    iPointer++;
                    lCounter++;
                }

                while (sWorkString.Length >= BYTESIZE)
                {
                    string sTemp = sWorkString.Substring(0, 8);
                    sWorkString = sWorkString.Substring(8);
                    sBuffer += (char)Bin2Dec(sTemp);
                    if (sBuffer.Length > iMinLen)
                    {
                        result += sBuffer;
                        sBuffer = "";
                    }
                }
            }

            sWTable = "";
            if (sWorkString.Length > 0)
            {
                sWorkString = (sWorkString + new string('0', 8)).Substring(0, 8);
                sBuffer += (char)Bin2Dec(sWorkString);
            }
            if (sBuffer.Length > 0)
            {
                result += sBuffer;
                sBuffer = "";
            }
            return result;
        }
        public string UNSQueeZeIT(string record, out long chkSum, int iMinLen = 1024)
        {
            long lCurrentWorkSize = 1;
            int iBitSize = 2;
            long lWorkingSize = 4;
            string sWTable = "";
            bool bCompleted = false;
            string sWorkString = "";
            string sBuffer = "";
            string result = "";
            chkSum = 0;

            while (!bCompleted)
            {
                // Fill sWorkString with more bits from record if needed
                GetChar(ref sWorkString, ref record, out bool bCharsDone);

                while (!bCompleted)
                {
                    // If not done, and sWorkString is running low, refill
                    if (!bCharsDone)
                    {
                        if (sWorkString.Length < 100)
                        {
                            GetChar(ref sWorkString, ref record, out bCharsDone);
                        }
                    }
                    else
                    {
                        if (sWorkString.Length <= 8)
                        {
                            if (sWorkString.All(c => c == '0'))
                            {
                                bCompleted = true;
                            }
                        }
                    }

                    if (!bCompleted)
                    {
                        string sWorkItem;
                        long lLenWorkItem;
                        long lPosInTable;

                        if (sWorkString.Length > 0 && sWorkString[0] == '1')
                        {
                            // Match found in Win
                            if (sWorkString.Length < iBitSize + 1)
                                break; // Not enough bits

                            string binIndex = sWorkString.Substring(1, iBitSize);
                            lPosInTable = Bin2Dec(binIndex);

                            // Remove used bits
                            sWorkString = sWorkString.Substring(iBitSize + 1);

                            // Find length of work item
                            int i = 1;
                            while (i < sWorkString.Length && sWorkString.Substring(i - 1, 1) != "1" || sWorkString.Substring(i - 1, 1) == "")
                                i++;

                            if (i < sWorkString.Length && sWorkString.Substring(i - 1, i) == "1")
                                lLenWorkItem = 1;
                            else
                                lLenWorkItem = Bin2Dec(sWorkString.Substring(i - 1, i));

                            // Remove length bits
                            sWorkString = sWorkString.Substring(2 * i - 1);

                            // Extract work item from table
                            if (lPosInTable - 1 + lLenWorkItem <= sWTable.Length)
                                sWorkItem = sWTable.Substring((int)lPosInTable - 1, (int)lLenWorkItem);
                            else
                                sWorkItem = "";

                        }
                        else
                        {
                            // Char passed as is
                            if (sWorkString.Length < 9)
                                break; // Not enough bits

                            sWorkItem = ((char)Bin2Dec(sWorkString.Substring(1, 8))).ToString();
                            sWorkString = sWorkString.Substring(9);
                            lLenWorkItem = 1;
                        }

                        sWTable += sWorkItem;
                        lCurrentWorkSize += lLenWorkItem;

                        sBuffer += sWorkItem;
                        if (sBuffer.Length > iMinLen)
                        {
                            result += sBuffer;
                            sBuffer = "";
                        }

                        if (lCurrentWorkSize >= lWorkingSize)
                        {
                            lWorkingSize *= 2;
                            iBitSize += 1;
                        }

                        chkSum += TotalAsciiValue(sWorkItem);
                    }
                }

                if (sBuffer.Length > 0)
                {
                    result += sBuffer;
                    sBuffer = "";
                }
            }

            return result;
        }

        //public string SqzCrypt(string sData, string sMasterPassword, string sKey, string password, bool bEncrypt)
        //{
        //    bool bDone = false;
        //    int j;
        //    string sTemp;
        //    int POS = 0;
        //    double lValue;
        //    double C, Xo, X, A, Y;
        //    int minlen = iMinLen;
        //    int L;
        //    long chkSum = 0;
        //    string CH, SCH, newCH, sRemainder;
        //    int iPassLen;

        //    // Build password character array
        //    iPassLen = password.Length;
        //    int[] sPassChrs = new int[iPassLen];
        //    for (j = 0; j < iPassLen; j++)
        //    {
        //        sTemp = password.Substring(j, 1);
        //        POS = sChars.IndexOf(sTemp);
        //        sPassChrs[j] = POS - (int)Math.Floor((double)POS / 10) * 10;
        //    }

        //    // Calculate C
        //    lValue = 0;
        //    for (j = 0; j < sKey.Length; j++)
        //    {
        //        lValue += (int)sKey[j];
        //    }
        //    C = Math.Sqrt(lValue);

        //    // Calculate Xo
        //    lValue = 0;
        //    string passChrsJoined = string.Join(((char)254).ToString(), sPassChrs);
        //    for (j = 0; j < passChrsJoined.Length; j++)
        //    {
        //        lValue += (int)passChrsJoined[j];
        //    }
        //    Xo = Math.Sqrt(lValue);

        //    // Calculate A
        //    lValue = 0;
        //    for (j = 0; j < sMasterPassword.Length; j++)
        //    {
        //        lValue += (int)sMasterPassword[j];
        //    }
        //    A = Math.Sqrt(lValue);

        //    Y = Xo * A;
        //    Y = Y + C;
        //    Y = Y % 93;
        //    Y = Math.Floor(Y);
        //    A = double.Parse(sPrimes[(int)Y - 1]);

        //    // Pad password to 10 chars
        //    password = (password + new string(' ', 10)).Substring(0, 10);

        //    bDone = false;
        //    minlen = iMinLen;
        //    L = sData.Length + 1;
        //    j = 0;
        //    X = Xo;
        //    POS = 0;

        //    StringBuilder result = new StringBuilder();

        //    while (!bDone)
        //    {
        //        CH = (j < sData.Length) ? sData.Substring(j, 1) : "";
        //        if (string.IsNullOrEmpty(CH))
        //        {
        //            bDone = true;
        //        }
        //        if (!bDone)
        //        {
        //            SCH = ((int)CH[0]).ToString();
        //            int schInt = int.Parse(SCH);
        //            if (bEncrypt)
        //            {
        //                chkSum += schInt;
        //            }

        //            X = X * A;
        //            X = X + C;

        //            // Track remainder
        //            sRemainder = "";
        //            string xStr = X.ToString();
        //            int dotIdx = xStr.IndexOf(".");
        //            if (dotIdx >= 0)
        //            {
        //                sRemainder = xStr.Substring(dotIdx);
        //            }
        //            double xMod = double.Parse((X % iMod).ToString() + sRemainder, System.Globalization.CultureInfo.InvariantCulture);
        //            double sRemVal = 0;
        //            if (!string.IsNullOrEmpty(sRemainder))
        //                double.TryParse(sRemainder, out sRemVal);
        //            X = xMod - Math.Round(sRemVal);

        //            POS++;
        //            if (POS > iPassLen)
        //                POS = 1;

        //            int passChr = sPassChrs[POS - 1];
        //            int newSchInt;
        //            if (bEncrypt)
        //            {
        //                newSchInt = schInt - ((int)X + passChr);
        //                if (newSchInt < 0)
        //                    newSchInt = 255 + newSchInt;
        //            }
        //            else
        //            {
        //                newSchInt = schInt + ((int)X + passChr);
        //                if (newSchInt >= 255)
        //                    newSchInt = newSchInt - 255;
        //            }
        //            newCH = ((char)newSchInt).ToString();
        //            if (!bEncrypt)
        //            {
        //                chkSum += schInt;
        //            }
        //            result.Append(newCH);

        //            if (j > minlen)
        //            {
        //                sData = sData.Substring(j + 1);
        //                j = 0;
        //            }
        //            else
        //            {
        //                j++;
        //            }
        //        }
        //    }
        //    return result.ToString();
        //}

        /// <summary>
        /// Fills sBinaryString with more bits from sRecord, using class-level iDecodePointer/iDecodeCounter.
        /// Sets bCharsDone = true if sRecord is exhausted.
        /// </summary>
        private void GetChar(ref string sBinaryString, ref string sRecord, out bool bCharsDone)
        {
            const int iMinLen = 100;
            bCharsDone = false;

            if (iDecodePointer == 0) iDecodePointer = 1;

            // Mask for 8 bits
            const int maskLen = 8;

            while (sBinaryString.Length <= iMinLen && !bCharsDone)
            {
                // Get next char from sRecord
                string sNextCh = (iDecodePointer - 1 < sRecord.Length) ? sRecord.Substring(iDecodePointer - 1, 1) : "";
                if (string.IsNullOrEmpty(sNextCh))
                {
                    bCharsDone = true;
                    iDecodePointer = 0;
                    break;
                }

                // Convert char to 8-bit binary and append
                sBinaryString += Dec2Bin((int)sNextCh[0]).PadLeft(maskLen, '0');

                // Move pointer/counter
                if (iDecodePointer > iMinLen)
                {
                    sRecord = sRecord.Substring(iDecodePointer);
                    iDecodePointer = 1;
                }
                else
                {
                    iDecodePointer++;
                    iDecodeCounter++;
                }

                // Progress bar code omitted
            }
        }
        /// <summary>
        /// Converts a binary string (e.g. "1011") to its decimal integer value.
        /// Returns 0 if the string is longer than 49 bits.
        /// </summary>
        private static long Bin2Dec(string sBinary)
        {
            if (string.IsNullOrEmpty(sBinary))
                return 0;

            if (sBinary.Length > 49)
                return 0; // Or throw an exception if you want to signal an error

            long result = 0;
            int len = sBinary.Length;
            for (int i = 0; i < len; i++)
            {
                // Check if the character is '1'
                if (sBinary[len - 1 - i] == '1')
                {
                    result += 1L << i;
                }
            }
            return result;
        }
        // Helper: Calculate total ASCII value of a string
        private static int TotalAsciiValue(string s)
        {
            int sum = 0;
            foreach (char c in s)
                sum += c;
            return sum;
        }
        private static string GetBinaryString(string sWorkItem, int bitSize, long lOldPosInTable)
        {
            if (sWorkItem.Length == 1)
            {
                // Single character: "0" + 8-bit binary of ASCII value
                return "0" + Dec2Bin((int)sWorkItem[0]).PadLeft(8, '0');
            }
            else
            {
                // Multi-character: "1" + position in table (bitSize bits) + length of work item (variable bits)
                string sPartBin = Dec2Bin(lOldPosInTable).PadLeft(bitSize, '0');
                string sLenWorkItemBin = Dec2Bin(sWorkItem.Length);
                sLenWorkItemBin = new string('0', sLenWorkItemBin.Length - 1) + sLenWorkItemBin;
                return "1" + sPartBin + sLenWorkItemBin;
            }
        }
        private static string Dec2Bin(long value)
        {
            // Handle large numbers as in VB: if value is too large, return "0"
            if (value < 0 || value.ToString().Length > 9)
                return "0";

            // Convert to hex string (uppercase, no "0x" prefix)
            string hexValue = value.ToString("X");

            // Build binary string using asBinTable
            StringBuilder binBuilder = new StringBuilder();
            foreach (char hexChar in hexValue)
            {
                // Convert hex char to int
                int nibble = Convert.ToInt32(hexChar.ToString(), 16);
                binBuilder.Append(asBinTable[nibble]);
            }

            // Remove leading zeros
            string binStr = binBuilder.ToString().TrimStart('0');
            if (string.IsNullOrEmpty(binStr))
                binStr = "0";

            return binStr;
        }
        public string LZWCompress(string input)
        {
            // Initialize the dictionary with single characters
            var dictionary = new Dictionary<string, int>();
            //for (int i = 0; i < 256; i++)
            //    dictionary[((char)i).ToString()] = i;
            for (int i = 0; i < sChars.Length; i++)
            {
                dictionary[sChars[i].ToString()] = i ; // Extend dictionary for custom characters
            }

            string w = string.Empty;
            var result = new List<int>();
            int dictSize = sChars.Length;

            foreach (char c in input)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    result.Add(dictionary[w]);
                    dictionary[wc] = dictSize++;
                    w = c.ToString();
                }
            }

            // Output the code for w
            if (!string.IsNullOrEmpty(w))
                result.Add(dictionary[w]);

            return string.Concat(result.Select(i => (char)i));
        }

        public string LZWDecompress(string compressed)
        {
            List<int> ints = compressed.Select(c => (int)c).ToList();
            // Initialize the dictionary with single characters
            var dictionary = new Dictionary<int, string>();
            //for (int i = 0; i < 256; i++)
            //    dictionary[i] = ((char)i).ToString();
            for (int i = 0; i < sChars.Length; i++)
            {
                dictionary[i] = sChars[i].ToString();
            }
            int dictSize = sChars.Length;
            string w = dictionary[ints[0]];
            var result = new StringBuilder(w);

            for (int i = 1; i < ints.Count; i++)
            {
                int k = ints[i];
                string entry;

                if (dictionary.ContainsKey(k))
                {
                    entry = dictionary[k];
                }
                else if (k == dictSize)
                {
                    entry = w + w[0];
                }
                else
                {
                    throw new ArgumentException("Bad compressed k: " + k);
                }

                result.Append(entry);

                // Add w+entry[0] to the dictionary
                dictionary[dictSize++] = w + entry[0];
                w = entry;
            }

            return result.ToString();
        }
    }
}