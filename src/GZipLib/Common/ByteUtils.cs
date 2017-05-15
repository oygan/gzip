namespace GZipLib.Common
{
    /// <summary>
    /// Утилитные функции для работы с байтами
    /// </summary>
    static internal class ByteUtils
    {
        /// <summary>
        /// Искать набор байтов в массиве
        /// </summary>
        /// <param name="searchIn">входной массив</param>
        /// <param name="searchBytes">последовательность байтов которые нужно найти</param>
        /// <param name="start">индекс с которого начать поиск</param>
        /// <returns>Найденная позиция. "-1" - последовательность не найдена</returns>
        public static int SearchByte(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }
                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }
                    }
                }
            }
            return found;
        }
    }
}