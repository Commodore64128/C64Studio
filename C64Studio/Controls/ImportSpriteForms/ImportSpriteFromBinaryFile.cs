﻿using C64Studio.Formats;
using C64Studio.Types;
using GR.Memory;
using RetroDevStudio;
using RetroDevStudio.Formats;
using RetroDevStudio.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static C64Studio.BaseDocument;

namespace C64Studio.Controls
{
  public partial class ImportSpriteFromBinaryFile : ImportSpriteFormBase
  {
    public ImportSpriteFromBinaryFile() :
      base( null )
    { 
    }



    public ImportSpriteFromBinaryFile( StudioCore Core ) :
      base( Core )
    {
      InitializeComponent();
    }



    public override bool HandleImport( SpriteProject Project, SpriteEditor Editor )
    {
      string filename;

      if ( !Editor.OpenFile( "Open sprite file", Types.Constants.FILEFILTER_SPRITE + Types.Constants.FILEFILTER_SPRITE_SPRITEPAD + Types.Constants.FILEFILTER_ALL, out filename ) )
      {
        return false;
      }
      return Editor.ImportSprites( filename, true, true );
    }



  }
}
