﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using SteamKit2;

namespace DepotDownloader
{
    static class Util
    {
        [DllImport( "libc" )]
        static extern int uname( IntPtr buf );

        static int _isMacOSX = -1;

        // Environment.OSVersion.Platform returns PlatformID.Unix under Mono on OS X
        // Code adapted from Mono: mcs/class/Managed.Windows.Forms/System.Windows.Forms/XplatUI.cs
        private static bool IsMacOSX()
        {
            if ( _isMacOSX != -1 )
                return _isMacOSX == 1;

            IntPtr buf = IntPtr.Zero;

            try
            {
                // The size of the utsname struct varies from system to system, but this _seems_ more than enough
                buf = Marshal.AllocHGlobal( 4096 );

                if ( uname( buf ) == 0 )
                {
                    string sys = Marshal.PtrToStringAnsi( buf );
                    if ( sys == "Darwin" )
                    {
                        _isMacOSX = 1;
                        return true;
                    }
                }
            }
            catch
            {
                // Do nothing?
            }
            finally
            {
                if ( buf != IntPtr.Zero )
                    Marshal.FreeHGlobal( buf );
            }

            _isMacOSX = 0;
            return false;
        }

        public static string GetSteamOS()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    return "windows";
                case PlatformID.MacOSX:
                    return "macos";
                case PlatformID.Unix:
                    return IsMacOSX() ? "macos" : "linux";
            }

            return "unknown";
        }

        public static string ReadPassword()
        {
            ConsoleKeyInfo keyInfo;
            StringBuilder password = new StringBuilder();

            do 
            {
                keyInfo = Console.ReadKey( true );

                if ( keyInfo.Key == ConsoleKey.Backspace )
                {
                    if ( password.Length > 0 )
                        password.Remove( password.Length - 1, 1 );
                    continue;
                }

                /* Printable ASCII characters only */
                char c = keyInfo.KeyChar;
                if ( c >= ' ' && c <= '~' )
                    password.Append( c );
            } while ( keyInfo.Key != ConsoleKey.Enter );

            return password.ToString();
        }

        // Validate a file against Steam3 Chunk data
        public static List<ProtoManifest.ChunkData> ValidateSteam3FileChecksums(FileStream fs, ProtoManifest.ChunkData[] chunkdata)
        {
            var neededChunks = new List<ProtoManifest.ChunkData>();
            int read;

            foreach (var data in chunkdata)
            {
                byte[] chunk = new byte[data.UncompressedLength];
                fs.Seek((long)data.Offset, SeekOrigin.Begin);
                read = fs.Read(chunk, 0, (int)data.UncompressedLength);

                byte[] tempchunk;
                if (read < data.UncompressedLength)
                {
                    tempchunk = new byte[read];
                    Array.Copy(chunk, 0, tempchunk, 0, read);
                }
                else
                {
                    tempchunk = chunk;
                }

                byte[] adler = AdlerHash(tempchunk);
                if (!adler.SequenceEqual(data.Checksum))
                {
                    neededChunks.Add(data);
                }
            }

            return neededChunks;
        }

        public static byte[] AdlerHash(byte[] input)
        {
            uint a = 0, b = 0;
            for (int i = 0; i < input.Length; i++)
            {
                a = (a + input[i]) % 65521;
                b = (b + a) % 65521;
            }
            return BitConverter.GetBytes(a | (b << 16));
        }

        public static byte[] SHAHash( byte[] input )
        {
            SHA1Managed sha = new SHA1Managed();

            byte[] output = sha.ComputeHash( input );

            sha.Clear();

            return output;
        }

        public static byte[] DecodeHexString( string hex )
        {
            if ( hex == null )
                return null;

            int chars = hex.Length;
            byte[] bytes = new byte[ chars / 2 ];

            for ( int i = 0 ; i < chars ; i += 2 )
                bytes[ i / 2 ] = Convert.ToByte( hex.Substring( i, 2 ), 16 );

            return bytes;
        }

        public static string EncodeHexString( byte[] input )
        {
            return input.Aggregate( new StringBuilder(),
                ( sb, v ) => sb.Append( v.ToString( "x2" ) )
                ).ToString();
        }

        public static byte[] AESDecryptECB( byte[] input, byte[] key )
        {
            using ( var aes = new RijndaelManaged() )
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.ECB;

                byte[] plainText = new byte[ input.Length ];
                int outLen = 0;
                                
                using ( var aesTransform = aes.CreateDecryptor( key, null ) )
                using ( var ms = new MemoryStream( input ) )
                using ( var cs = new CryptoStream( ms, aesTransform, CryptoStreamMode.Read ) )
                {
                    outLen = cs.Read( plainText, 0, plainText.Length );
                }
                
                byte[] output = new byte[ outLen ];
                Array.Copy( plainText, 0, output, 0, output.Length );
                return output;
            }            
        }
    }
}
