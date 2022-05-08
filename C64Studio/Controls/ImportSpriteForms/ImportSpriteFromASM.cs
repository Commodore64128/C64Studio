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
  public partial class ImportSpriteFromASM : ImportSpriteFormBase
  {
    public ImportSpriteFromASM() :
      base( null )
    { 
    }



    public ImportSpriteFromASM( StudioCore Core ) :
      base( Core )
    {
      InitializeComponent();
    }



    public override bool HandleImport( SpriteProject CharSet, SpriteEditor Editor )
    {
      var data = Util.FromASMData( editInput.Text );
      if ( data == null )
      {
        return false;
      }

      Editor.ImportFromData( data );
      return true;
    }



    private void editInput_KeyPress( object sender, KeyPressEventArgs e )
    {
      if ( ( ModifierKeys == Keys.Control )
      &&   ( e.KeyChar == 1 ) )
      {
        editInput.SelectAll();
        e.Handled = true;
      }
    }



  }
}
