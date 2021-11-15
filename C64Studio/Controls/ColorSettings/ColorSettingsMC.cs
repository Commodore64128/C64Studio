﻿using RetroDevStudio;
using RetroDevStudio.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace C64Studio.Controls
{
  public partial class ColorSettingsMC : ColorSettingsBase
  {
    public override int CustomColor
    {
      get
      {
        return _CustomColor;
      }
      set
      {
        _CustomColor = value;
        if ( comboCharColor != null )
        {
          comboCharColor.SelectedIndex = value;
        }
      }
    }



    public override ColorType SelectedColor
    {
      get
      {
        return _CurrentColorType;
      }
      set
      {
        switch ( _CurrentColorType )
        {
          case ColorType.BACKGROUND:
            radioBackground.Checked = true;
            break;
          case ColorType.MULTICOLOR_1:
            radioMultiColor1.Checked = true;
            break;
          case ColorType.MULTICOLOR_2:
            radioMulticolor2.Checked = true;
            break;
          case ColorType.CUSTOM_COLOR:
            radioCharColor.Checked = true;
            break;
          default:
            return;
        }
        _CurrentColorType = value;
      }
    }



    public ColorSettingsMC() :
      base( null, null, 0 )
    { 
    }



    public ColorSettingsMC( StudioCore Core, ColorSettings Colors, int CustomColor ) :
      base( Core, Colors, CustomColor )
    {
      InitializeComponent();

      for ( int i = 0; i < Colors.Palette.NumColors; ++i )
      {
        comboCharColor.Items.Add( i.ToString( "d2" ) );
        comboBackground.Items.Add( i.ToString( "d2" ) );
        comboMulticolor1.Items.Add( i.ToString( "d2" ) );
        comboMulticolor2.Items.Add( i.ToString( "d2" ) );
      }
      comboBackground.SelectedIndex = Colors.BackgroundColor;
      comboMulticolor1.SelectedIndex = Colors.MultiColor1;
      comboMulticolor2.SelectedIndex = Colors.MultiColor2;
      comboCharColor.SelectedIndex = CustomColor;

      radioCharColor.Checked = true;
    }



    private void comboColor_DrawItem( object sender, DrawItemEventArgs e )
    {
      ComboBox combo = (ComboBox)sender;

      Core?.Theming.DrawSingleColorComboBox( combo, e, Colors.Palette );
    }



    private void comboCharColor_DrawItem( object sender, DrawItemEventArgs e )
    {
      ComboBox combo = (ComboBox)sender;

      Core?.Theming.DrawMultiColorComboBox( combo, e, Colors.Palette );
    }



    private void comboBackground_SelectedIndexChanged( object sender, EventArgs e )
    {
      Colors.BackgroundColor = comboBackground.SelectedIndex;
      RaiseColorsModifiedEvent( ColorType.BACKGROUND );
    }



    private void comboMulticolor1_SelectedIndexChanged( object sender, EventArgs e )
    {
      Colors.MultiColor1 = comboMulticolor1.SelectedIndex;
      RaiseColorsModifiedEvent( ColorType.MULTICOLOR_1 );
    }



    private void comboMulticolor2_SelectedIndexChanged( object sender, EventArgs e )
    {
      Colors.MultiColor2 = comboMulticolor2.SelectedIndex;
      RaiseColorsModifiedEvent( ColorType.MULTICOLOR_2 );
    }



    private void comboCharColor_SelectedIndexChanged( object sender, EventArgs e )
    {
      CustomColor = comboCharColor.SelectedIndex;
      RaiseColorsModifiedEvent( ColorType.CUSTOM_COLOR );
    }



    private void btnExchangeColors_Click( object sender, EventArgs e )
    {
      contextMenuExchangeColors.Show( btnExchangeColors, new Point( 0, btnExchangeColors.Height ) );
    }



    private void radioBackground_CheckedChanged( object sender, EventArgs e )
    {
      _CurrentColorType = ColorType.BACKGROUND;
      RaiseColorSelectedEvent();
    }



    private void radioMultiColor1_CheckedChanged( object sender, EventArgs e )
    {
      _CurrentColorType = ColorType.MULTICOLOR_1;
      RaiseColorSelectedEvent();
    }



    private void radioMulticolor2_CheckedChanged( object sender, EventArgs e )
    {
      _CurrentColorType = ColorType.MULTICOLOR_2;
      RaiseColorSelectedEvent();
    }



    private void radioCharColor_CheckedChanged( object sender, EventArgs e )
    {
      _CurrentColorType = ColorType.CUSTOM_COLOR;
      RaiseColorSelectedEvent();
    }



    public override void ColorChanged( ColorType Color, int Value )
    {
      switch ( Color )
      {
        case ColorType.BACKGROUND:
          comboBackground.SelectedIndex = Value;
          break;
        case ColorType.MULTICOLOR_1:
          comboMulticolor1.SelectedIndex = Value;
          break;
        case ColorType.MULTICOLOR_2:
          comboMulticolor2.SelectedIndex = Value;
          break;
        case ColorType.CUSTOM_COLOR:
          comboCharColor.SelectedIndex = Value;
          break;
      }
    }



    private void exchangeMultiColor1WithMultiColor2ToolStripMenuItem_Click( object sender, EventArgs e )
    {
      RaiseColorsExchangedEvent( ColorType.MULTICOLOR_1, ColorType.MULTICOLOR_2 );
    }



    private void exchangeMultiColor1WithBGColorToolStripMenuItem_Click( object sender, EventArgs e )
    {
      RaiseColorsExchangedEvent( ColorType.MULTICOLOR_1, ColorType.BACKGROUND );
    }



    private void exchangeMultiColor2WithBGColorToolStripMenuItem_Click( object sender, EventArgs e )
    {
      RaiseColorsExchangedEvent( ColorType.MULTICOLOR_2, ColorType.BACKGROUND );
    }



    private void exchangeCharColorWithBGColorToolStripMenuItem_Click( object sender, EventArgs e )
    {
      RaiseColorsExchangedEvent( ColorType.CUSTOM_COLOR, ColorType.BACKGROUND );
    }



    private void exchangeCustomColorWithMultiColor1ToolStripMenuItem_Click( object sender, EventArgs e )
    {
      RaiseColorsExchangedEvent( ColorType.MULTICOLOR_1, ColorType.CUSTOM_COLOR );
    }



    private void exchangeCustomColorWithMultiColor2ToolStripMenuItem_Click( object sender, EventArgs e )
    {
      RaiseColorsExchangedEvent( ColorType.MULTICOLOR_2, ColorType.CUSTOM_COLOR );
    }



  }
}