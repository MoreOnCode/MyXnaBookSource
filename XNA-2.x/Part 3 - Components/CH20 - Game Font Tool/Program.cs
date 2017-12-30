// Program.cs
// The command-line front-end for the FontMaker class.

using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GameFontUtility
{
    class GameFontUtility
    {
        // individual font properties
        private static string m_FontName = "Courier New";
        private static float m_FontSize = 10.0f;
        private static bool m_Bold = false;
        private static bool m_Italic = false;
        private static bool m_Underline = false;
        private static bool m_Strikeout = false;
        
        // if the user doesn't specify characters to encode,
        // encode the default (ASCII) character subset
        private static bool m_EncodeDefault = true;
        
        // output image filename
        private static string m_Filename = "font.png";

        // advanced settings that affect how GDI(+) 
        // renders text in the selected font. See MSDN 
        // for more information about how these settings 
        // affect your text
        private static TextRenderingHint m_RenderHint = 
            TextRenderingHint.AntiAliasGridFit;
        private static StringFormat m_StringFormat = 
            StringFormat.GenericTypographic;

        // the only instance of the font encoder
        private static FontMaker m_FontMaker = new FontMaker();
        
        // parse the command line to set internal options
        public static bool ParseOptions(string[] args)
        {
            // no paramers? show usage and exit.
            if (args.Length == 0)
            {
                Usage();
                return false;
            }

            // check each argument
            for (int i = 0; i < args.Length; i++)
            {
                // the current token
                string curr = args[i];

                // the next token, if any
                string next = "";
                if (i < args.Length - 1)
                {
                    next = args[i + 1];
                }

                // allow '/' or '-', but use '-' internally
                if (curr.StartsWith("/"))
                {
                    curr = "-" + curr.Substring(1);
                }

                // if argument is a valid switch
                if (curr.StartsWith("-"))
                {
                    // case-insensitive comparisons
                    switch (curr.ToUpper())
                    {
                        case "-FONTNAME":
                        case "-NAME":
                            m_FontName = next;
                            i++;
                            break;

                        case "-FONTSIZE":
                        case "-SIZE":
                            try
                            {
                                m_FontSize = float.Parse(next);
                                i++;
                            }
                            catch (Exception ex)
                            {
                                ReportError(ex);
                                return false;
                            }
                            break;
                        
                        case "-BOLD":
                        case "-B":
                            m_Bold = true;
                            break;
                        
                        case "-ITALIC":
                        case "-I":
                            m_Italic = true;
                            break;
                        
                        case "-STRIKEOUT":
                        case "-S":
                            m_Strikeout = true;
                            break;

                        case "-UNDERLINE":
                        case "-U":
                            m_Underline = true;
                            break;

                        case "-FILENAME":
                        case "-FILE":
                        case "-OUT":
                            m_Filename = next;
                            i++;
                            break;

                        case "-ENCODENONE":
                        case "-ENCODECLEAR":
                            m_EncodeDefault = false;
                            m_FontMaker.ClearCharsToEncode();
                            break;

                        case "-ENCODEASCII":
                            m_EncodeDefault = false;
                            m_FontMaker.DefaultCharsToEncode(
                                FontMaker.EncodeDefault.ASCII);
                            break;

                        case "-ENCODESCRIPT":
                        case "-SCRIPT":
                            m_EncodeDefault = false;
                            EncodeScript(next);
                            i++;
                            break;

                        case "-FONTDIALOG":
                        case "-DIALOG":
                        case "-DLG":
                            FontDialog dlg = new FontDialog();
                            if (DialogResult.OK == dlg.ShowDialog())
                            {
                                Font f = dlg.Font;
                                m_Bold = f.Bold;
                                m_Italic = f.Italic;
                                m_Strikeout = f.Strikeout;
                                m_Underline = f.Underline;
                                m_FontSize = f.Size;
                                m_FontName = f.Name;
                            }
                            break;

                        case "-TEXTRENDERINGHINT":
                        case "-RENDERINGHINT":
                        case "-RENDERHINT":
                        case "-TEXTHINT":
                        case "-HINT":
                        case "-TRH":
                            switch (next.ToUpper())
                            {
                                case "ANTIALIAS":
                                    m_RenderHint =
                                        TextRenderingHint
                                        .AntiAlias;
                                    break;
                                case "ANTIALIASGRIDFIT":
                                    m_RenderHint =
                                        TextRenderingHint
                                        .AntiAliasGridFit;
                                    break;
                                case "CLEARTYPEGRIDFIT":
                                    m_RenderHint =
                                        TextRenderingHint
                                        .ClearTypeGridFit;
                                    break;
                                case "SINGLEBITPERPIXEL":
                                    m_RenderHint =
                                        TextRenderingHint
                                        .SingleBitPerPixel;
                                    break;
                                case "SINGLEBITPERPIXELGRIDFIT":
                                    m_RenderHint =
                                        TextRenderingHint
                                        .SingleBitPerPixelGridFit;
                                    break;
                                case "SYSTEMDEFAULT":
                                    m_RenderHint =
                                        TextRenderingHint
                                        .SystemDefault;
                                    break;
                            }
                            i++;
                            break;

                        case "-STRINGFORMAT":
                        case "-FORMAT":
                            switch (next.ToUpper())
                            {
                                case "GENERICTYPOGRAPHIC":
                                    m_StringFormat = 
                                        StringFormat
                                        .GenericTypographic;
                                    break;
                                case "GENERICDEFAULT":
                                    m_StringFormat =
                                        StringFormat
                                        .GenericDefault;
                                    break;
                            }
                            i++;
                            break;

                        case "-HELP":
                        case "-?":
                            Usage();
                            return false;
                    }
                }
            }
            return true;
        }

        // this method will read a text file, and mark
        // the unique characters within for encoding. 
        // especially useful if your unicode charaters
        // can't be typed at the command prompt.
        public static void EncodeScript(string file)
        {
            try
            {
                // open the script file
                TextReader reader = new StreamReader(file);

                // read the file line-by-line
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    // for each character in the line
                    char[] chars = line.ToCharArray();
                    foreach (char c in chars)
                    {
                        // add it to the list of characters
                        // to be encoded. the FontMaker class
                        // will resolve any duplicates for us.
                        m_FontMaker.AddCharToEncode(c);
                    }
                }
                // free resources
                reader.Close();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        // display a message in a popup dialog
        public static void ReportError(string msg)
        {
            MessageBox.Show(msg);
        }

        // overload for string version of method with same name
        public static void ReportError(Exception ex)
        {
            ReportError(ex.Message + "\n" + ex.StackTrace);
        }

        // actually encode the font into a texture
        public static void MakeFont()
        {
            if (m_EncodeDefault)
            {
                // user didn't specify what chars to encode
                // use the default ASCII subset
                m_FontMaker.ClearCharsToEncode();
                m_FontMaker.DefaultCharsToEncode(
                    FontMaker.EncodeDefault.ASCII);
            }

            // relay command line options to FontMaker class,
            // then encode th font
            m_FontMaker.RenderFormat = m_StringFormat;
            m_FontMaker.RenderHint = m_RenderHint;
            m_FontMaker.Encode(
                new Font(m_FontName, m_FontSize, MakeFontStyle()),
                m_Filename);
        }

        // simple way to turn all our bool's into a FontStyle
        static FontStyle MakeFontStyle()
        {
            return
                (m_Bold ? FontStyle.Bold : FontStyle.Regular) |
                (m_Italic ? FontStyle.Italic : FontStyle.Regular) |
                (m_Strikeout ? FontStyle.Strikeout : FontStyle.Regular) |
                (m_Underline ? FontStyle.Underline : FontStyle.Regular);
        }

        // list options for the user
        static void Usage()
        {
            Console.Out.WriteLine(@"GameFontUtility - 
Converts a Windows font into a bitmap font, for use in games.

  -fontname {name}     font option, name
  -fontsize {size}     font option, size
  -bold                font option, bold
  -italic              font option, italic
  -strikeout           font option, strikeout
  -underline           font option, underline
  -filename {file}     The file to receive the new texture image
  -encodeclear         Clear the list of characters to encode
  -encodeascii         Encode a useful subset of ASCII
  -encodescript {file} Parse specified file encoding each char found
  -dialog              Use Windows font dialog to choose font
  -help                Display this message
  -stringformat {opt}  Select a render format from the following:
                          generictypographic, genericdefault
  -renderhint {opt}    Select a render hint from the following:
                          antialias, antialiasgridfit, cleartypegridfit
                          singlebitperpixel, singlebitperpixelgridfit,
                          systemdefault

EXAMPLE: GameFontUtility -dialog -filename test.png");
        }

        // the entry point for this application
        static void Main(string[] args)
        {
            if (ParseOptions(args))
            {
                MakeFont();
            }
        }
    }
}
