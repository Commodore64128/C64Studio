﻿using System;
using System.Collections.Generic;

namespace FastColoredTextBoxNS
{
  /// <summary>
  /// Insert single char
  /// </summary>
  /// <remarks>This operation includes also insertion of new line and removing char by backspace</remarks>
  public class InsertCharCommand : UndoableCommand
  {
    public char c;
    char deletedChar = '\x0';

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ts">Underlaying textbox</param>
    /// <param name="c">Inserting char</param>
    public InsertCharCommand( TextSource ts, char c )
      : base( ts )
    {
      this.c = c;
    }

    /// <summary>
    /// Undo operation
    /// </summary>
    public override void Undo()
    {
      ts.OnTextChanging();

      var previousPlace = ts.CurrentTB.Selection.Start;

      switch ( c )
      {
        case '\n':
          MergeLines( sel.Start.iLine, ts );
          break;
        case '\r':
          break;
        case '\b':
          ts.CurrentTB.Selection.Start = lastSel.Start;
          char cc = '\x0';
          if ( deletedChar != '\x0' )
          {
            ts.CurrentTB.ExpandBlock( ts.CurrentTB.Selection.Start.iLine );

            InsertChar( deletedChar, ref cc, ts );

            DumpLine( ts, ts.CurrentTB.Selection.Start.iLine );
          }
          break;
        case '\t':
          ts.CurrentTB.ExpandBlock( sel.Start.iLine );

          if ( ts.CurrentTB.AllowTabs )
          {
            char    cdummy = c;

            ts.CurrentTB.Selection.Start = new Place( ts.CurrentTB.AdjustXPosForTabsRight( sel.Start.iLine, sel.Start.iChar ), sel.Start.iLine );

            InsertChar( '\b', ref cdummy, ts );

            sel.Start = ts.CurrentTB.Selection.Start;
            /*
            int     tabPos = ts.CurrentTB.AdjustXPosForTabsRight( sel.Start.iLine, sel.Start.iChar );
            if ( tabPos == sel.Start.iChar )
            {
              tabPos += ts.CurrentTB.TabLength;
            }

            for ( int i = sel.Start.iChar; i < tabPos - sel.Start.iChar; i++ )
              ts[sel.Start.iLine].RemoveAt( i );*/
          }
          else
          {
            for ( int i = sel.FromX; i < lastSel.FromX; i++ )
              ts[sel.Start.iLine].RemoveAt( sel.Start.iChar );
          }
          ts.CurrentTB.Selection.Start = sel.Start;
          break;
        default:
          ts.CurrentTB.ExpandBlock( sel.Start.iLine );

          // tabs may be shifting (arghghghghghg)
          if ( ts.CurrentTB.AllowTabs )
          {
            for ( int j = sel.Start.iChar + 1; j < ts[sel.Start.iLine].Count; ++j )
            {
              if ( ts[sel.Start.iLine][j].c == '\t' )
              {
                if ( ( j % ts.CurrentTB.TabLength ) == 0 )
                {
                  // a full tab, needs to be removed and replaced by a single tab char
                  ts[sel.Start.iLine].RemoveRange( j, ts.CurrentTB.TabLength - 1 );
                }
                else
                {
                  // tab grows backwards
                  ts[sel.Start.iLine].Insert( j, new Char( '\t' ) );
                }
                break;
              }
            }
          }
          ts[sel.Start.iLine].RemoveAt( sel.Start.iChar );
          ts.CurrentTB.Selection.Start = sel.Start;
          break;
      }

      var postPlace = ts.CurrentTB.Selection.End;

      ts.CurrentTB.OnTextDeleted( new Range( ts.CurrentTB, previousPlace.iChar, previousPlace.iLine, postPlace.iChar, postPlace.iLine ) );

      ts.NeedRecalc( new TextSource.TextChangedEventArgs( sel.Start.iLine, sel.Start.iLine ) );

      base.Undo();
    }

    /// <summary>
    /// Execute operation
    /// </summary>
    public override void Execute()
    {
      ts.CurrentTB.ExpandBlock( ts.CurrentTB.Selection.Start.iLine );
      string s = c.ToString();
      ts.OnTextChanging( ref s );
      if ( s.Length == 1 )
        c = s[0];

      if ( String.IsNullOrEmpty( s ) )
        throw new ArgumentOutOfRangeException();

      var previousPlace = ts.CurrentTB.Selection.Start;

      if ( ts.Count == 0 )
        InsertLine( ts );
      InsertChar( c, ref deletedChar, ts );

      var postPlace = ts.CurrentTB.Selection.End;

      ts.CurrentTB.OnTextInserted( new Range( ts.CurrentTB, previousPlace.iChar, previousPlace.iLine, postPlace.iChar, postPlace.iLine ) );

      ts.NeedRecalc( new TextSource.TextChangedEventArgs( ts.CurrentTB.Selection.Start.iLine, ts.CurrentTB.Selection.Start.iLine ) );
      base.Execute();
    }



    public static void DumpLine( TextSource ts, int line )
    {
      /*
      System.Text.StringBuilder   sb = new System.Text.StringBuilder();

      for ( int i = 0; i < ts[line].Count; ++i )
      {
        sb.Append( ts[line][i].c );
      }
      Debug.Log( sb.ToString().Replace( '\t', 'x' ) );
       */
    }



    internal static int RemoveChar( TextSource ts, int LineIndex, int CharIndex, ref char deletedChar )
    {
      var tb = ts.CurrentTB;

      deletedChar = ts[LineIndex][CharIndex].c;
      int     numCharsToDelete = 1;
      /*
      if ( ( tb.AllowTabs )
      &&   ( deletedChar == '\t' ) )*/
      {
        int origCharIndex = CharIndex;
        int newPos = tb.AdjustXPosForTabs( LineIndex, CharIndex );

        numCharsToDelete = CharIndex + 1 - newPos;
        CharIndex -= numCharsToDelete - 1;

        string    deTabbed = ts[LineIndex].ToText( origCharIndex + 1 );

        // remove all after current position
        ts[LineIndex].RemoveRange( CharIndex, ts[LineIndex].Count - CharIndex );

        ts[LineIndex].AddTabified( deTabbed );

        return numCharsToDelete;
      }

      //bool changeMade = false;

      /*
      // if we deleted the right-most char (or the trailing tab), and a tab follows up, collapse it
      if ( ( tb.AllowTabs )
      &&   ( ( ( CharIndex + 1 ) % tb.TabLength ) == 0 )
      &&   ( ( CharIndex + 1 ) < ts[LineIndex].Count )
      &&   ( ts[LineIndex][CharIndex + 1].c == '\t' ) )
      {
        // fill our tab slot with tabs
        for ( int i = 0; i < numCharsToDelete; ++i )
        {
          ts[LineIndex][CharIndex + 1 - numCharsToDelete + i] = new Char( '\t' );
        }
        // remove follow up tab
        ts[LineIndex].RemoveRange( CharIndex + 1, tb.TabLength );
        changeMade = true;
      }

      // if we deleted a char in a line where a tab is to the right, fill in one tab char (or a full block if tab collapses)
      //   but only if there is not already a full tab block!
      if ( tb.AllowTabs )
      {
        // is there a tab in our "block"?
        var curLine = ts[LineIndex];

        for ( int i = CharIndex; i < curLine.Count; ++i )
        {
          if ( curLine[i].c == '\t' )
          {
            // there's a tab!

            // move chars to left
            for ( int j = CharIndex; j < curLine.Count - 1; ++j )
            {
              if ( ( ts[LineIndex][j + 1].c == '\t' )
              &&   ( ( ( j + 1 ) % tb.TabLength ) == 0 ) )
              {
                // tab starts at full block, no copying!
              }
              else
              {
                ts[LineIndex][j] = new Char( ts[LineIndex][j + 1].c );
                if ( ts[LineIndex][j].c == '\t' )
                {
                  // copyied tab char, we're done
                  break;
                }
              }
            }
            changeMade = true;
            break;
          }
        }
      }

      if ( !changeMade )
      {
        ts[LineIndex].RemoveRange( CharIndex + 1 - numCharsToDelete, numCharsToDelete );
      }
      return numCharsToDelete;*/
    }



    internal static void InsertChar( char c, ref char deletedChar, TextSource ts )
    {
      var tb = ts.CurrentTB;

      switch ( c )
      {
        case '\n':
          if ( !ts.CurrentTB.AllowInsertRemoveLines )
            throw new ArgumentOutOfRangeException( "Cant insert this char in ColumnRange mode" );
          if ( ts.Count == 0 )
            InsertLine( ts );
          InsertLine( ts );
          break;
        case '\r':
          break;
        case '\b'://backspace
          if ( ( tb.Selection.Start.iChar == 0 )
          &&   ( tb.Selection.Start.iLine == 0 ) )
          {
            return;
          }
          if ( tb.Selection.Start.iChar == 0 )
          {
            if ( !ts.CurrentTB.AllowInsertRemoveLines )
              throw new ArgumentOutOfRangeException( "Cant insert this char in ColumnRange mode" );
            if ( tb.LineInfos[tb.Selection.Start.iLine - 1].VisibleState != VisibleState.Visible )
              tb.ExpandBlock( tb.Selection.Start.iLine - 1 );
            deletedChar = '\n';
            MergeLines( tb.Selection.Start.iLine - 1, ts );
          }
          else
          {
            int     numCharsToDelete = RemoveChar( ts, tb.Selection.Start.iLine, tb.Selection.Start.iChar - 1, ref deletedChar );
            tb.Selection.Start = new Place( tb.Selection.Start.iChar - numCharsToDelete, tb.Selection.Start.iLine );

            //Debug.Log( "Backspace at " + ( tb.Selection.Start.iChar + numCharsToDelete ) );
            DumpLine( ts, tb.Selection.Start.iLine );
          }
          break;
        case '\t':
          if ( ( tb.AllowTabs )
          &&   ( !tb.ConvertTabsToSpaces ) )
          {
            //Debug.Log( "Insert tab at " + tb.Selection.Start.iChar );
            int spaceCountNextTabStop = tb.TabLength - ( tb.Selection.Start.iChar % tb.TabLength );
            if ( spaceCountNextTabStop == 0 )
              spaceCountNextTabStop = tb.TabLength;

            string    deTabbed = "\t" + ts[tb.Selection.Start.iLine].ToText( tb.Selection.Start.iChar );

            ts[tb.Selection.Start.iLine].RemoveRange( tb.Selection.Start.iChar, ts[tb.Selection.Start.iLine].Count - tb.Selection.Start.iChar );
            ts[tb.Selection.Start.iLine].AddTabified( deTabbed );



            /*
            // do we shift a tab?
            if ( ( tb.Selection.Start.iChar < ts[tb.Selection.Start.iLine].Count )
            &&   ( ts[tb.Selection.Start.iLine][tb.Selection.Start.iChar].c == '\t' ) )
            {
              // yes, insert a full tab behind
              //Debug.Log( "Shifting another tab" );
              // ...make it a full tab
              for ( int i = 0; i < tb.TabLength; i++ )
              {
                ts[tb.Selection.Start.iLine].Insert( tb.Selection.Start.iChar + spaceCountNextTabStop, new Char( '\t' ) );
              }
            }
            else
            {
              for ( int i = 0; i < spaceCountNextTabStop; i++ )
                ts[tb.Selection.Start.iLine].Insert( tb.Selection.Start.iChar, new Char( '\t' ) );
            }*/
            tb.Selection.Start = new Place( tb.Selection.Start.iChar + spaceCountNextTabStop, tb.Selection.Start.iLine );

            //DumpLine( ts, tb.Selection.Start.iLine );
          }
          else if ( tb.ConvertTabsToSpaces )
          {
            int spaceCountNextTabStop = tb.TabLength - ( tb.Selection.Start.iChar % tb.TabLength );
            if ( spaceCountNextTabStop == 0 )
              spaceCountNextTabStop = tb.TabLength;

            for ( int i = 0; i < spaceCountNextTabStop; i++ )
              ts[tb.Selection.Start.iLine].Insert( tb.Selection.Start.iChar, new Char( ' ' ) );

            tb.Selection.Start = new Place( tb.Selection.Start.iChar + spaceCountNextTabStop, tb.Selection.Start.iLine );
          }
          else
          {
            int spaceCountNextTabStop = tb.TabLength - ( tb.Selection.Start.iChar % tb.TabLength );
            if ( spaceCountNextTabStop == 0 )
              spaceCountNextTabStop = tb.TabLength;

            for ( int i = 0; i < spaceCountNextTabStop; i++ )
              ts[tb.Selection.Start.iLine].Insert( tb.Selection.Start.iChar, new Char( ' ' ) );

            tb.Selection.Start = new Place( tb.Selection.Start.iChar + spaceCountNextTabStop, tb.Selection.Start.iLine );
          }
          break;
        default:
          // tabs may be shifted! (URGHHHARGHHHH)
          if ( tb.AllowTabs )
          {
            string deTabbed = ts[tb.Selection.Start.iLine].ToText( tb.Selection.Start.iChar );

            ts[tb.Selection.Start.iLine].RemoveRange( tb.Selection.Start.iChar, ts[tb.Selection.Start.iLine].Count - tb.Selection.Start.iChar );

            deTabbed = c + deTabbed;

            ts[tb.Selection.Start.iLine].AddTabified( deTabbed );


            /*
            // re-tab (only need to adjust first tab?)
            for ( int j = tb.Selection.Start.iChar; j < ts[tb.Selection.Start.iLine].Count; ++j )
            {
              if ( ts[tb.Selection.Start.iLine][j].c == '\t' )
              {
                // if tab is only 1 char need to insert new full tab (-1, because existing tab char is shifted already)
                if ( ( j % tb.TabLength ) == tb.TabLength - 1 )
                {
                  for ( int i = 0; i < tb.TabLength - 1; ++i )
                  {
                    ts[tb.Selection.Start.iLine].Insert( j + i, new Char( '\t' ) );
                  }
                }
                else
                {
                  // remove one tab character
                  ts[tb.Selection.Start.iLine].RemoveAt( j );
                }
                break;
              }
            }*/
          }
          else
          {
            ts[tb.Selection.Start.iLine].Insert( tb.Selection.Start.iChar, new Char( c ) );
          }
          tb.Selection.Start = new Place( tb.Selection.Start.iChar + 1, tb.Selection.Start.iLine );
          break;
      }
    }

    internal static void InsertLine( TextSource ts )
    {
      var tb = ts.CurrentTB;

      if ( !tb.Multiline && tb.LinesCount > 0 )
        return;

      if ( ts.Count == 0 )
        ts.InsertLine( 0, ts.CreateLine() );
      else
        BreakLines( tb.Selection.Start.iLine, tb.Selection.Start.iChar, ts );

      tb.Selection.Start = new Place( 0, tb.Selection.Start.iLine + 1 );
      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );
    }

    /// <summary>
    /// Merge lines i and i+1
    /// </summary>
    internal static void MergeLines( int i, TextSource ts )
    {
      var tb = ts.CurrentTB;

      if ( i + 1 >= ts.Count )
        return;
      tb.ExpandBlock( i );
      tb.ExpandBlock( i + 1 );
      int pos = ts[i].Count;
      //
      /*
      if(ts[i].Count == 0)
          ts.RemoveLine(i);
      else*/
      if ( ts[i + 1].Count == 0 )
        ts.RemoveLine( i + 1 );
      else
      {
        if ( tb.AllowTabs )
        {
          string    deTabbed = ts[i + 1].ToText( 0 );
          ts[i].AddTabified( deTabbed );
        }
        else
        {
          ts[i].AddRange( ts[i + 1] );
        }
        ts.RemoveLine( i + 1 );
      }
      tb.Selection.Start = new Place( pos, i );
      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );
    }

    internal static void BreakLines( int iLine, int pos, TextSource ts )
    {
      Line newLine = ts.CreateLine();

      if ( ts.CurrentTB.AllowTabs )
      {
        string    deTabbed = ts[iLine].ToText( pos );

        ts[iLine].RemoveRange( pos, ts[iLine].Count - pos );
        newLine.AddTabified( deTabbed );
      }
      else
      {
        for ( int i=pos; i < ts[iLine].Count; i++ )
        {
          newLine.Add( ts[iLine][i] );
        }
        ts[iLine].RemoveRange( pos, ts[iLine].Count - pos );
      }
      //
      ts.InsertLine( iLine + 1, newLine );
    }

    public override UndoableCommand Clone()
    {
      return new InsertCharCommand( ts, c );
    }
  }

  /// <summary>
  /// Insert text
  /// </summary>
  public class InsertTextCommand : UndoableCommand
  {
    public string InsertedText;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ts">Underlaying textbox</param>
    /// <param name="insertedText">Text for inserting</param>
    public InsertTextCommand( TextSource ts, string insertedText )
      : base( ts )
    {
      this.InsertedText = insertedText;
    }

    /// <summary>
    /// Undo operation
    /// </summary>
    public override void Undo()
    {
      ts.CurrentTB.Selection.Start = sel.Start;
      ts.CurrentTB.Selection.End = lastSel.Start;
      ts.OnTextChanging();
      ClearSelectedCommand.ClearSelected( ts );
      base.Undo();
    }

    /// <summary>
    /// Execute operation
    /// </summary>
    public override void Execute()
    {
      ts.OnTextChanging( ref InsertedText );
      InsertText( InsertedText, ts );
      base.Execute();
    }

    internal static void InsertText( string insertedText, TextSource ts )
    {
      var tb = ts.CurrentTB;

      var startPos = new Place( tb.Selection.Start.iChar, tb.Selection.Start.iLine );
      var endPos = new Place( tb.Selection.Start.iChar, tb.Selection.Start.iLine );
      try
      {
        tb.Selection.BeginUpdate();
        char cc = '\x0';

        if ( ts.Count == 0 )
        {
          InsertCharCommand.InsertLine( ts );
          tb.Selection.Start = Place.Empty;
        }
        tb.ExpandBlock( tb.Selection.Start.iLine );
        var len = insertedText.Length;
        for ( int i = 0; i < len; i++ )
        {
          var c = insertedText[i];
          if ( c == '\r' && ( i >= len - 1 || insertedText[i + 1] != '\n' ) )
            InsertCharCommand.InsertChar( '\n', ref cc, ts );
          else
            InsertCharCommand.InsertChar( c, ref cc, ts );
        }
        ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );

        endPos = new Place( tb.Selection.Start.iChar, tb.Selection.Start.iLine );
      }
      finally
      {
        tb.Selection.EndUpdate();
        tb.OnTextInserted( new Range( tb, startPos, endPos ) );
      }
    }

    public override UndoableCommand Clone()
    {
      return new InsertTextCommand( ts, InsertedText );
    }
  }

  /// <summary>
  /// Insert text into given ranges
  /// </summary>
  public class ReplaceTextCommand : UndoableCommand
  {
    string insertedText;
    List<Range> ranges;
    List<string> prevText = new List<string>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ts">Underlaying textbox</param>
    /// <param name="ranges">List of ranges for replace</param>
    /// <param name="insertedText">Text for inserting</param>
    public ReplaceTextCommand( TextSource ts, List<Range> ranges, string insertedText )
      : base( ts )
    {
      //sort ranges by place
      ranges.Sort( ( r1, r2 ) =>
          {
            if ( r1.Start.iLine == r2.Start.iLine )
              return r1.Start.iChar.CompareTo( r2.Start.iChar );
            return r1.Start.iLine.CompareTo( r2.Start.iLine );
          } );
      //
      this.ranges = ranges;
      this.insertedText = insertedText;
      lastSel = sel = new RangeInfo( ts.CurrentTB.Selection );
    }

    /// <summary>
    /// Undo operation
    /// </summary>
    public override void Undo()
    {
      var tb = ts.CurrentTB;

      ts.OnTextChanging();
      tb.BeginUpdate();

      tb.Selection.BeginUpdate();
      for ( int i = 0; i < ranges.Count; i++ )
      {
        tb.Selection.Start = ranges[i].Start;
        for ( int j = 0; j < insertedText.Length; j++ )
          tb.Selection.GoRight( true );
        ClearSelected( ts );
        InsertTextCommand.InsertText( prevText[prevText.Count - i - 1], ts );
      }
      tb.Selection.EndUpdate();
      tb.EndUpdate();

      if ( ranges.Count > 0 )
        ts.OnTextChanged( ranges[0].Start.iLine, ranges[ranges.Count - 1].End.iLine );

      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );
    }

    /// <summary>
    /// Execute operation
    /// </summary>
    public override void Execute()
    {
      var tb = ts.CurrentTB;
      prevText.Clear();

      ts.OnTextChanging( ref insertedText );

      tb.Selection.BeginUpdate();
      tb.BeginUpdate();
      for ( int i = ranges.Count - 1; i >= 0; i-- )
      {
        tb.Selection.Start = ranges[i].Start;
        tb.Selection.End = ranges[i].End;
        prevText.Add( tb.Selection.Text );
        ClearSelected( ts );
        if ( insertedText != "" )
          InsertTextCommand.InsertText( insertedText, ts );
      }
      if ( ranges.Count > 0 )
        ts.OnTextChanged( ranges[0].Start.iLine, ranges[ranges.Count - 1].End.iLine );
      tb.EndUpdate();
      tb.Selection.EndUpdate();
      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );

      lastSel = new RangeInfo( tb.Selection );
    }

    public override UndoableCommand Clone()
    {
      return new ReplaceTextCommand( ts, new List<Range>( ranges ), insertedText );
    }

    internal static void ClearSelected( TextSource ts )
    {
      var tb = ts.CurrentTB;

      tb.Selection.Normalize();

      Place start = tb.Selection.Start;
      Place end = tb.Selection.End;
      int fromLine = Math.Min( end.iLine, start.iLine );
      int toLine = Math.Max( end.iLine, start.iLine );
      int fromChar = tb.Selection.FromX;
      int toChar = tb.Selection.ToX;
      if ( fromLine < 0 )
        return;
      //
      if ( fromLine == toLine )
        ts[fromLine].RemoveRange( fromChar, toChar - fromChar );
      else
      {
        ts[fromLine].RemoveRange( fromChar, ts[fromLine].Count - fromChar );
        ts[toLine].RemoveRange( 0, toChar );
        ts.RemoveLine( fromLine + 1, toLine - fromLine - 1 );
        InsertCharCommand.MergeLines( fromLine, ts );
      }
    }
  }

  /// <summary>
  /// Clear selected text
  /// </summary>
  public class ClearSelectedCommand : UndoableCommand
  {
    string deletedText;

    /// <summary>
    /// Construstor
    /// </summary>
    /// <param name="ts">Underlaying textbox</param>
    public ClearSelectedCommand( TextSource ts )
      : base( ts )
    {
    }

    /// <summary>
    /// Undo operation
    /// </summary>
    public override void Undo()
    {
      ts.CurrentTB.Selection.Start = new Place( sel.FromX, Math.Min( sel.Start.iLine, sel.End.iLine ) );
      ts.OnTextChanging();
      InsertTextCommand.InsertText( deletedText, ts );
      ts.OnTextChanged( sel.Start.iLine, sel.End.iLine );
      ts.CurrentTB.Selection.Start = sel.Start;
      ts.CurrentTB.Selection.End = sel.End;
    }

    /// <summary>
    /// Execute operation
    /// </summary>
    public override void Execute()
    {
      var tb = ts.CurrentTB;

      string temp = null;
      ts.OnTextChanging( ref temp );
      if ( temp == "" )
        throw new ArgumentOutOfRangeException();

      deletedText = tb.Selection.Text;
      var deletedRange = new RangeInfo( tb.Selection );

      ClearSelected( ts );
      lastSel = new RangeInfo( tb.Selection );
      ts.OnTextDeleted( new Range( tb, deletedRange.Start, deletedRange.End ) );
      ts.OnTextChanged( lastSel.Start.iLine, lastSel.Start.iLine );
    }



    static void RemoveRange( TextSource ts, int fromLine, int fromChar, int toChar )
    {
      var tb = ts.CurrentTB;

      if ( tb.AllowTabs )
      {
        //InsertCharCommand.DumpLine( ts, fromLine );

        System.Diagnostics.Debug.WriteLine( $"Removing range from {fromChar} to {toChar}" );

        // remove backwards to properly handle tabs
        for ( int i = toChar - 1; i >= fromChar; )
        {
          char  dummy = 'ö';

          string    deTabbed = ts[fromLine].ToText( fromChar );
          string    fullTabbed = ts[fromLine].ToVirtualText(0);
          deTabbed = deTabbed.Replace( '\t', '*' );
          fullTabbed = fullTabbed.Replace( '\t', '*' );
          System.Diagnostics.Debug.WriteLine( $"Removing at pos {i}, text is           {fullTabbed}" );

          /*
          if ( ( i == toChar )
          &&   ( i >= ts[fromLine].Count ) )
          {
            --i;
            continue;
          }*/

          int     delta = InsertCharCommand.RemoveChar( ts, fromLine, i, ref dummy );

          deTabbed = ts[fromLine].ToText( fromChar );
          fullTabbed = ts[fromLine].ToVirtualText(0);
          fullTabbed = fullTabbed.Replace( '\t', '*' );
          deTabbed = deTabbed.Replace( '\t', '*' );
          System.Diagnostics.Debug.WriteLine( $"Removed {delta} characters, text is now     {fullTabbed}" );

          i -= delta;
        }
      }
      else
      {
        ts[fromLine].RemoveRange( fromChar, toChar - fromChar );
      }
    }



    internal static void ClearSelected( TextSource ts )
    {
      var tb = ts.CurrentTB;

      Place start = tb.Selection.Start;
      Place end = tb.Selection.End;
      int fromLine = Math.Min( end.iLine, start.iLine );
      int toLine = Math.Max( end.iLine, start.iLine );
      int fromChar = tb.Selection.FromX;
      int toChar = tb.Selection.ToX;
      if ( fromLine < 0 )
        return;
      //
      if ( fromLine == toLine )
      {
        RemoveRange( ts, fromLine, fromChar, toChar );
      }
      else
      {
        RemoveRange( ts, fromLine, fromChar, ts[fromLine].Count );
        //ts[fromLine].RemoveRange( fromChar, ts[fromLine].Count - fromChar );
        //ts[toLine].RemoveRange( 0, toChar );
        RemoveRange( ts, toLine, 0, toChar );
        ts.RemoveLine( fromLine + 1, toLine - fromLine - 1 );
        InsertCharCommand.MergeLines( fromLine, ts );
      }
      //
      tb.Selection.Start = new Place( fromChar, fromLine );
      //
      ts.NeedRecalc( new TextSource.TextChangedEventArgs( fromLine, toLine ) );
    }

    public override UndoableCommand Clone()
    {
      return new ClearSelectedCommand( ts );
    }
  }

  /// <summary>
  /// Replaces text
  /// </summary>
  public class ReplaceMultipleTextCommand : UndoableCommand
  {
    List<ReplaceRange> ranges;
    List<string> prevText = new List<string>();

    public class ReplaceRange
    {
      public Range ReplacedRange
      {
        get;
        set;
      }
      public String ReplaceText
      {
        get;
        set;
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ts">Underlaying textsource</param>
    /// <param name="ranges">List of ranges for replace</param>
    public ReplaceMultipleTextCommand( TextSource ts, List<ReplaceRange> ranges )
      : base( ts )
    {
      //sort ranges by place
      ranges.Sort( ( r1, r2 ) =>
      {
        if ( r1.ReplacedRange.Start.iLine == r2.ReplacedRange.Start.iLine )
          return r1.ReplacedRange.Start.iChar.CompareTo( r2.ReplacedRange.Start.iChar );
        return r1.ReplacedRange.Start.iLine.CompareTo( r2.ReplacedRange.Start.iLine );
      } );
      //
      this.ranges = ranges;
      lastSel = sel = new RangeInfo( ts.CurrentTB.Selection );
    }

    /// <summary>
    /// Undo operation
    /// </summary>
    public override void Undo()
    {
      var tb = ts.CurrentTB;

      ts.OnTextChanging();

      tb.Selection.BeginUpdate();
      for ( int i = 0; i < ranges.Count; i++ )
      {
        tb.Selection.Start = ranges[i].ReplacedRange.Start;
        for ( int j = 0; j < ranges[i].ReplaceText.Length; j++ )
          tb.Selection.GoRight( true );
        ClearSelectedCommand.ClearSelected( ts );
        var prevTextIndex = ranges.Count - 1 - i;
        InsertTextCommand.InsertText( prevText[prevTextIndex], ts );
        ts.OnTextChanged( ranges[i].ReplacedRange.Start.iLine, ranges[i].ReplacedRange.Start.iLine );
      }
      tb.Selection.EndUpdate();

      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );
    }

    /// <summary>
    /// Execute operation
    /// </summary>
    public override void Execute()
    {
      var tb = ts.CurrentTB;
      prevText.Clear();

      ts.OnTextChanging();

      tb.Selection.BeginUpdate();
      for ( int i = ranges.Count - 1; i >= 0; i-- )
      {
        tb.Selection.Start = ranges[i].ReplacedRange.Start;
        tb.Selection.End = ranges[i].ReplacedRange.End;
        prevText.Add( tb.Selection.Text );
        ClearSelectedCommand.ClearSelected( ts );
        InsertTextCommand.InsertText( ranges[i].ReplaceText, ts );
        ts.OnTextChanged( ranges[i].ReplacedRange.Start.iLine, ranges[i].ReplacedRange.End.iLine );
      }
      tb.Selection.EndUpdate();
      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );

      lastSel = new RangeInfo( tb.Selection );
    }

    public override UndoableCommand Clone()
    {
      return new ReplaceMultipleTextCommand( ts, new List<ReplaceRange>( ranges ) );
    }
  }

  /// <summary>
  /// Removes lines
  /// </summary>
  public class RemoveLinesCommand : UndoableCommand
  {
    List<int> iLines;
    List<string> prevText = new List<string>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ts">Underlaying textbox</param>
    /// <param name="iLines">List of ranges for replace</param>
    public RemoveLinesCommand( TextSource ts, List<int> iLines )
      : base( ts )
    {
      //sort iLines
      iLines.Sort();
      //
      this.iLines = iLines;
      lastSel = sel = new RangeInfo( ts.CurrentTB.Selection );
    }

    /// <summary>
    /// Undo operation
    /// </summary>
    public override void Undo()
    {
      var tb = ts.CurrentTB;

      ts.OnTextChanging();

      var insertedRange = new Range( tb, 0, iLines[0], 0, iLines[iLines.Count - 1] + 1 );
      tb.Selection.BeginUpdate();
      //tb.BeginUpdate();
      for ( int i = 0; i < iLines.Count; i++ )
      {
        var iLine = iLines[i];

        if ( iLine < ts.Count )
          tb.Selection.Start = new Place( 0, iLine );
        else
          tb.Selection.Start = new Place( ts[ts.Count - 1].Count, ts.Count - 1 );

        InsertCharCommand.InsertLine( ts );
        tb.Selection.Start = new Place( 0, iLine );
        var text = prevText[prevText.Count - i - 1];
        InsertTextCommand.InsertText( text, ts );
        ts[iLine].IsChanged = true;
        if ( iLine < ts.Count - 1 )
          ts[iLine + 1].IsChanged = true;
        else
          ts[iLine - 1].IsChanged = true;
        if ( text.Trim() != string.Empty )
          ts.OnTextChanged( iLine, iLine );
      }
      //tb.EndUpdate();
      tb.Selection.EndUpdate();

      tb.OnTextInserted( insertedRange );

      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );
    }

    /// <summary>
    /// Execute operation
    /// </summary>
    public override void Execute()
    {
      var tb = ts.CurrentTB;
      prevText.Clear();

      ts.OnTextChanging();

      tb.Selection.BeginUpdate();
      for ( int i = iLines.Count - 1; i >= 0; i-- )
      {
        var iLine = iLines[i];

        prevText.Add( ts[iLine].Text );//backward
        ts.RemoveLine( iLine );
        //ts.OnTextChanged(ranges[i].Start.iLine, ranges[i].End.iLine);
      }
      tb.Selection.Start = new Place( 0, 0 );
      tb.Selection.EndUpdate();
      ts.NeedRecalc( new TextSource.TextChangedEventArgs( 0, 1 ) );

      lastSel = new RangeInfo( tb.Selection );
    }

    public override UndoableCommand Clone()
    {
      return new RemoveLinesCommand( ts, new List<int>( iLines ) );
    }
  }

  /// <summary>
  /// Wrapper for multirange commands
  /// </summary>
  public class MultiRangeCommand : UndoableCommand
  {
    private UndoableCommand cmd;
    private Range range;
    private List<UndoableCommand> commandsByRanges = new List<UndoableCommand>();

    public MultiRangeCommand( UndoableCommand command )
      : base( command.ts )
    {
      this.cmd = command;
      range = ts.CurrentTB.Selection.Clone();
    }

    public override void Execute()
    {
      commandsByRanges.Clear();
      var prevSelection = range.Clone();
      var iChar = -1;
      var iStartLine = prevSelection.Start.iLine;
      var iEndLine = prevSelection.End.iLine;
      ts.CurrentTB.Selection.ColumnSelectionMode = false;
      ts.CurrentTB.Selection.BeginUpdate();
      ts.CurrentTB.BeginUpdate();
      ts.CurrentTB.AllowInsertRemoveLines = false;
      try
      {
        if ( cmd is InsertTextCommand )
          ExecuteInsertTextCommand( ref iChar, ( cmd as InsertTextCommand ).InsertedText );
        else
          if ( cmd is InsertCharCommand && ( cmd as InsertCharCommand ).c != '\x0' && ( cmd as InsertCharCommand ).c != '\b' )//if not DEL or BACKSPACE
            ExecuteInsertTextCommand( ref iChar, ( cmd as InsertCharCommand ).c.ToString() );
          else
            ExecuteCommand( ref iChar );
      }
      catch ( ArgumentOutOfRangeException )
      {
      }
      finally
      {
        ts.CurrentTB.AllowInsertRemoveLines = true;
        ts.CurrentTB.EndUpdate();

        ts.CurrentTB.Selection = range;
        if ( iChar >= 0 )
        {
          ts.CurrentTB.Selection.Start = new Place( iChar, iStartLine );
          ts.CurrentTB.Selection.End = new Place( iChar, iEndLine );
        }
        ts.CurrentTB.Selection.ColumnSelectionMode = true;
        ts.CurrentTB.Selection.EndUpdate();
      }
    }

    private void ExecuteInsertTextCommand( ref int iChar, string text )
    {
      var lines = text.Split( '\n' );
      var iLine = 0;
      foreach ( var r in range.GetSubRanges( true ) )
      {
        var line = ts.CurrentTB[r.Start.iLine];
        var lineIsEmpty = r.End < r.Start && line.StartSpacesCount == line.Count;
        if ( !lineIsEmpty )
        {
          var insertedText = lines[iLine % lines.Length];
          if ( r.End < r.Start && insertedText != "" )
          {
            //add forwarding spaces
            insertedText = new string( ' ', r.Start.iChar - r.End.iChar ) + insertedText;
            r.Start = r.End;
          }
          ts.CurrentTB.Selection = r;
          var c = new InsertTextCommand( ts, insertedText );
          c.Execute();
          if ( ts.CurrentTB.Selection.End.iChar > iChar )
            iChar = ts.CurrentTB.Selection.End.iChar;
          commandsByRanges.Add( c );
        }
        iLine++;
      }
    }

    private void ExecuteCommand( ref int iChar )
    {
      foreach ( var r in range.GetSubRanges( false ) )
      {
        ts.CurrentTB.Selection = r;
        var c = cmd.Clone();
        c.Execute();
        if ( ts.CurrentTB.Selection.End.iChar > iChar )
          iChar = ts.CurrentTB.Selection.End.iChar;
        commandsByRanges.Add( c );
      }
    }

    public override void Undo()
    {
      ts.CurrentTB.BeginUpdate();
      ts.CurrentTB.Selection.BeginUpdate();
      try
      {
        for ( int i = commandsByRanges.Count - 1; i >= 0; i-- )
          commandsByRanges[i].Undo();
      }
      finally
      {
        ts.CurrentTB.Selection.EndUpdate();
        ts.CurrentTB.EndUpdate();
      }
      ts.CurrentTB.Selection = range.Clone();
      ts.CurrentTB.OnTextChanged( range );
      ts.CurrentTB.OnSelectionChanged();
      ts.CurrentTB.Selection.ColumnSelectionMode = true;
    }

    public override UndoableCommand Clone()
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Remembers current selection and restore it after Undo
  /// </summary>
  public class SelectCommand : UndoableCommand
  {
    public SelectCommand( TextSource ts )
      : base( ts )
    {
    }

    public override void Execute()
    {
      //remember selection
      lastSel = new RangeInfo( ts.CurrentTB.Selection );
    }

    protected override void OnTextChanged( bool invert )
    {
    }

    public override void Undo()
    {
      //restore selection
      ts.CurrentTB.Selection = new Range( ts.CurrentTB, lastSel.Start, lastSel.End );
    }

    public override UndoableCommand Clone()
    {
      var result = new SelectCommand( ts );
      if ( lastSel != null )
        result.lastSel = new RangeInfo( new Range( ts.CurrentTB, lastSel.Start, lastSel.End ) );
      return result;
    }
  }
}
