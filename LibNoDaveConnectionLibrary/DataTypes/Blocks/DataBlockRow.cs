﻿/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using DotNetSiemensPLCToolBoxLibrary.General;


namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public class DataBlockRow : IDataRow, INotifyPropertyChanged
    {
        public virtual List<IDataRow> Children { get; protected set; }

        public virtual S7DataRowType DataType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Comment { get; set; }

        public virtual IDataRow Parent { get; set; }

        public virtual ByteBitAddress BlockAddress { get; protected set; }

        public virtual Block PlcBlock { get; set; }

        public string StructuredName
        {
            get
            {
                if (Parent != null)
                {
                    if (string.IsNullOrEmpty((Parent).StructuredName)) return Name;
                    return (Parent).StructuredName + "." + Name;
                }
                return "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsArray { get; set; }

        public List<int> ArrayStart { get; set; }
        
        public List<int> ArrayStop { get; set; }
       
        public int GetArrayLines()
        {
            int arrcnt = 1;
            if (ArrayStart != null)
                for (int n = 0; n < ArrayStart.Count; n++)
                {
                    arrcnt *= ArrayStop[n] - ArrayStart[n] + 1;
                }
            return arrcnt;
        }

        public int DataTypeBlockNumber { get; set; } //When the Type is SFB, FB or UDT, this contains the Number!

        public int StringSize { get; set; } //Only Relevant for String     

        public string DataTypeAsString
        {
            get
            {
                string retVal = "";
                if (IsArray)
                {
                    retVal += "ARRAY [";
                    for (int n = 0; n < ArrayStart.Count; n++)
                    {
                        retVal += ArrayStart[n].ToString() + ".." + ArrayStop[n].ToString();
                        if (n < ArrayStart.Count - 1)
                            retVal += ",";
                    }
                    retVal += "] OF ";
                }
                retVal += DataType.ToString();

                if (DataType == S7DataRowType.FB || DataType == S7DataRowType.UDT || DataType == S7DataRowType.SFB)
                    retVal += DataTypeBlockNumber.ToString();
                if (DataType == S7DataRowType.STRING)
                    retVal += "[" + StringSize.ToString() + "]";

                return retVal;
            }
        }
    }
}
