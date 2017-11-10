﻿using C64Studio;
using C64Studio.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;



namespace C64Ass
{
  class Program
  {

    static int Main( string[] args )
    {
      var configReader = new ArgumentEvaluator();
      var config = configReader.CheckParams( args );
      if ( config == null )
      {
        return 1;
      }
      config.Assembler = C64Studio.Types.AssemblerType.C64_STUDIO;

      var parser = new ASMFileParser();

      var projectConfig = new ProjectConfig();
      // TODO - add defines if given

      string  fullPath = System.IO.Path.GetFullPath( config.InputFile );

      if ( string.IsNullOrEmpty( config.OutputFile ) )
      {
        // provide a default
        config.OutputFile = GR.Path.RenameExtension( config.InputFile, ".prg" );
        config.TargetType = C64Studio.Types.CompileTargetType.PRG;
      }

      bool result = parser.ParseFile( fullPath, "", projectConfig, config );
      if ( !result )
      {
        System.Console.WriteLine( "Parsing the file failed:" );

        DisplayOutput( parser );
        return 1;
      }

      // default to plain
      C64Studio.Types.CompileTargetType compileTargetType = C64Studio.Types.CompileTargetType.PLAIN;
      // command line given target type overrides everything
      if ( config.TargetType != C64Studio.Types.CompileTargetType.NONE )
      {
        compileTargetType = config.TargetType;
      }
      else if ( parser.CompileTarget != C64Studio.Types.CompileTargetType.NONE )
      {
        compileTargetType = parser.CompileTarget;
      }
      config.TargetType = compileTargetType;

      if ( !parser.Assemble( config ) )
      {
        System.Console.WriteLine( "Assembling the output failed" );
        DisplayOutput( parser );
        return 1;
      }
      DisplayOutput( parser );
      if ( !GR.IO.File.WriteAllBytes( config.OutputFile, parser.AssembledOutput.Assembly ) )
      {
        System.Console.WriteLine( "Failed to write output file" );
        return 1;
      }
      return 0;
    }



    private static void DisplayOutput( ASMFileParser Parser )
    {
      foreach ( var entry in Parser.Messages )
      {
        string    file;
        int       lineIndex;

        if ( Parser.ASMFileInfo.FindTrueLineSource( entry.Key, out file, out lineIndex ) )
        {
          System.Console.WriteLine( file + "(" + lineIndex + "): " + entry.Value.Code.ToString() + " - " + entry.Value.Message );
        }
        else
        {
          System.Console.WriteLine( entry.Value.Code.ToString() + " - " + entry.Value.Message );
        }
      }
    }
  }
}
