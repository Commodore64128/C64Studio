﻿using RetroDevStudio;
using RetroDevStudio.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;



namespace RetroDevStudio.Documents
{
  public partial class LabelExplorer : BaseDocument
  {
    public System.Windows.Forms.TreeNode      NodeRoot = null;
    private Project                           LabelExplorerProject = null;
    private GR.Collections.MultiMap<string,SymbolInfo>       LabelExplorerTokens = new GR.Collections.MultiMap<string, SymbolInfo>();

    private DocumentInfo                      ActiveDocumentInfo = null;
    private Types.ASM.FileInfo                ActiveASMFileInfo = null;

    private Dictionary<Types.ASM.FileInfo,GR.Collections.Map<string, bool>> _ExpandedNodesPerProject = new Dictionary<Types.ASM.FileInfo, GR.Collections.Map<string, bool>>();
    private Dictionary<string,Types.ASM.FileInfo>     _FileInfoPerFileCache = new Dictionary<string, Types.ASM.FileInfo>();



    public LabelExplorer()
    {
      InitializeComponent();

      GR.Image.DPIHandler.ResizeControlsForDPI( this );

      NodeRoot = new System.Windows.Forms.TreeNode( "All Labels" );
      NodeRoot.ImageIndex = 0;
      NodeRoot.SelectedImageIndex = 0;
      treeProject.Nodes.Add( NodeRoot );
    }



    private void treeProject_NodeMouseDoubleClick( object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e )
    {
      SymbolInfo tokenInfo = (SymbolInfo)e.Node.Tag;
      if ( tokenInfo == null )
      {
        if ( e.Node == NodeRoot )
        {
          // project node
          return;
        }
        //// zone node
        //MainForm.OpenDocumentAndGotoLine( tokenInfo.DocumentFilename, tokenInfo.LocalLineIndex );
        return;
      }
      Core.Navigating.OpenDocumentAndGotoLine( LabelExplorerProject, Core.Navigating.FindDocumentInfoByPath( tokenInfo.DocumentFilename ), tokenInfo.LocalLineIndex, tokenInfo.CharIndex, tokenInfo.Length );
    }



    private void treeProject_NodeMouseClick( object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e )
    {
      if ( ( e.Button == System.Windows.Forms.MouseButtons.Right )
      &&   ( e.Node != null ) )
      {
        if ( LabelExplorerProject == null )
        {
          return;
        }
      }
    }



    public override void OnApplicationEvent( ApplicationEvent Event )
    {
      switch ( Event.EventType )
      {
        case ApplicationEvent.Type.PROJECT_OPENED:
          break;
        case ApplicationEvent.Type.DOCUMENT_CLOSED:
          if ( Event.Doc.ASMFileInfo != null )
          {
            if ( _ExpandedNodesPerProject.ContainsKey( Event.Doc.ASMFileInfo ) )
            {
              _ExpandedNodesPerProject.Remove( Event.Doc.ASMFileInfo );
            }
          }
          if ( ActiveDocumentInfo == Event.Doc )
          {
            ActiveDocumentInfo = null;
            NodeRoot.Nodes.Clear();
          }
          break;
        case ApplicationEvent.Type.PROJECT_CLOSED:

          break;
      }
    }



    private void StoreOpenNodes()
    {
      if ( ActiveDocumentInfo != null )
      {
        if ( !_ExpandedNodesPerProject.ContainsKey( ActiveDocumentInfo.ASMFileInfo ) )
        {
          _ExpandedNodesPerProject.Add( ActiveDocumentInfo.ASMFileInfo, new GR.Collections.Map<string, bool>() );
        }
        var expandedNodesEntry = _ExpandedNodesPerProject[ActiveDocumentInfo.ASMFileInfo];
        expandedNodesEntry.Clear();
        foreach ( TreeNode node in NodeRoot.Nodes )
        {
          expandedNodesEntry[node.Text] = node.IsExpanded;
        }
      }
    }



    public void RefreshFromDocument( BaseDocument Doc )
    {
      if ( ( Doc == null )
      ||   ( Doc.DocumentInfo.FullPath == null ) )
      {
        return;
      }

      if ( ActiveASMFileInfo == Doc.DocumentInfo.ASMFileInfo )
      {
        // nothing to do
        ActiveDocumentInfo    = Doc.DocumentInfo;
        LabelExplorerProject  = Doc.DocumentInfo.Project;
        return;
      }

      if ( InvokeRequired )
      {
        Invoke( new MainForm.DocCallback( RefreshFromDocument ), new object[] { Doc } );
        return;
      }

      if ( _FileInfoPerFileCache.TryGetValue( Doc.DocumentInfo.FullPath, out Types.ASM.FileInfo cachedASMFileInfo ) )
      {
        if ( cachedASMFileInfo != Doc.DocumentInfo.ASMFileInfo )
        {
          _ExpandedNodesPerProject.Remove( cachedASMFileInfo );
        }
        _FileInfoPerFileCache.Remove( Doc.DocumentInfo.FullPath );
      }
      _FileInfoPerFileCache.Add( Doc.DocumentInfo.FullPath, Doc.DocumentInfo.ASMFileInfo );

      StoreOpenNodes();

      ActiveDocumentInfo    = Doc.DocumentInfo;
      LabelExplorerProject  = Doc.DocumentInfo.Project;

      ActiveASMFileInfo     = Doc.DocumentInfo.ASMFileInfo;
      LabelExplorerTokens   = Doc.DocumentInfo.KnownTokens;
      
      RefreshNodes();
    }



    private void RefreshNodes()
    {
      treeProject.BeginUpdate();
      NodeRoot.Nodes.Clear();

      if ( ActiveASMFileInfo == null )
      {
        treeProject.EndUpdate();
        return;
      }

      if ( !_ExpandedNodesPerProject.ContainsKey( ActiveASMFileInfo ) )
      {
        _ExpandedNodesPerProject.Add( ActiveASMFileInfo, new GR.Collections.Map<string, bool>() );
      }
      var expandedNodes = _ExpandedNodesPerProject[ActiveASMFileInfo];

      IList<SymbolInfo>     sortedTokens = null;

      // sort by line number
      if ( Core.Settings.LabelExplorerSortByIndex )
      {
        GR.Collections.MultiMap<int, SymbolInfo> sortedTokensInner = new GR.Collections.MultiMap<int, SymbolInfo>();
        foreach ( KeyValuePair<string, SymbolInfo> token in LabelExplorerTokens )
        {
          if ( !string.IsNullOrEmpty( Core.Settings.LabelExplorerFilter ) )
          {
            if ( token.Key.ToUpper().Contains( Core.Settings.LabelExplorerFilter.ToUpper() ) )
            {
              sortedTokensInner.Add( token.Value.LineIndex, token.Value );
            }
          }
          else
          {
            sortedTokensInner.Add( token.Value.LineIndex, token.Value );
          }
          
        }
        sortedTokens = sortedTokensInner.Values;
      }
      else
      {
        GR.Collections.MultiMap<string, SymbolInfo> sortedTokensInner = new GR.Collections.MultiMap<string, SymbolInfo>();
        foreach ( KeyValuePair<string, SymbolInfo> token in LabelExplorerTokens )
        {
          if ( !string.IsNullOrEmpty( Core.Settings.LabelExplorerFilter ) )
          {
            if ( token.Key.ToUpper().Contains( Core.Settings.LabelExplorerFilter.ToUpper() ) )
            {
              sortedTokensInner.Add( token.Key.ToUpper(), token.Value );
            }
          }
          else
          {
            sortedTokensInner.Add( token.Key.ToUpper(), token.Value );
          }
        }
        sortedTokens = sortedTokensInner.Values;
      }

      string curZone = "";

      System.Windows.Forms.TreeNode parentNode = new System.Windows.Forms.TreeNode();
      parentNode = new System.Windows.Forms.TreeNode();
      parentNode.Text = "Global Zone";
      parentNode.ImageIndex = parentNode.SelectedImageIndex = 0;
      NodeRoot.Nodes.Add( parentNode );
      System.Windows.Forms.TreeNode globalZone = parentNode;

      var zoneNodes = new Dictionary<string, TreeNode>();
      zoneNodes.Add( parentNode.Text, globalZone );

      // add zone nodes first
      foreach ( var token in sortedTokens )
      {
        if ( token.Type == SymbolInfo.Types.ZONE )
        {
          if ( zoneNodes.ContainsKey( token.Zone ) )
          {
            continue;
          }
          var node = new System.Windows.Forms.TreeNode();

          node.Text = token.Name;
          node.Tag = token;

          parentNode = new System.Windows.Forms.TreeNode();
          parentNode.Text = token.Zone;
          parentNode.ImageIndex = parentNode.SelectedImageIndex = 0;
          NodeRoot.Nodes.Add( parentNode );
          parentNode.Tag = token;

          zoneNodes.Add( parentNode.Text, parentNode );
        }
      }

      // now add child nodes
      parentNode = globalZone;
      foreach ( var token in sortedTokens )
      {
        if ( token.Type == SymbolInfo.Types.ZONE )
        {
          curZone = token.Zone;
          parentNode = zoneNodes[token.Zone];
          continue;
        }
        if ( token.Type == SymbolInfo.Types.TEMP_LABEL )
        {
          continue;
        }

        var   node = new System.Windows.Forms.TreeNode();
        bool  addNode = true;
        bool  addToGlobalNode = false;

        node.Text = token.Name;
        node.Tag = token;

        switch ( token.Type )
        {
          case SymbolInfo.Types.VARIABLE_NUMBER:
            node.Text += " = Number";
            break;
          case SymbolInfo.Types.VARIABLE_STRING:
            node.Text += " = String";
            break;
          case SymbolInfo.Types.VARIABLE_ARRAY:
            node.Text += " = Array";
            break;
          case SymbolInfo.Types.VARIABLE_INTEGER:
            node.Text += " = Integer";
            break;
          case SymbolInfo.Types.CONSTANT_STRING:
            node.Text += " = " + token.String;
            break;
          case SymbolInfo.Types.CONSTANT_1:
          case SymbolInfo.Types.CONSTANT_2:
            node.ImageIndex = node.SelectedImageIndex = 2;
            node.Text += " = $" + token.AddressOrValue.ToString( "X4" );
            break;
          case SymbolInfo.Types.LABEL:
            if ( ( token.Name.Contains( "." ) )
            &&   ( !Core.Settings.LabelExplorerShowLocalLabels ) )
            {
              addNode = false;
            }
            if ( ( token.Name.StartsWith( RetroDevStudio.Parser.ASMFileParser.InternalLabelPrefix ) )
            &&   ( !Core.Settings.LabelExplorerShowShortCutLabels ) )
            {
              addNode = false;
            }
            node.ImageIndex = node.SelectedImageIndex = 1;
            node.Text += " = $" + token.AddressOrValue.ToString( "X4" );
            break;
          case SymbolInfo.Types.PREPROCESSOR_LABEL:
          case SymbolInfo.Types.PREPROCESSOR_CONSTANT_1:
          case SymbolInfo.Types.PREPROCESSOR_CONSTANT_2:
            node.ImageIndex = node.SelectedImageIndex = 3;
            node.Text += " = $" + token.AddressOrValue.ToString( "X4" );
            break;
          case SymbolInfo.Types.UNKNOWN:
            break;
        }

        if ( !addNode )
        {
          continue;
        }

        if ( !token.Name.StartsWith( curZone + "." ) )
        {
          addToGlobalNode = true;
        }
        int     dotPos = token.Name.IndexOf( '.' );
        string  tokenZone = token.Zone;
        if ( ( dotPos != -1 )
        &&   ( dotPos > 0 ) )
        {
          tokenZone = token.Name.Substring( 0, dotPos );
          addToGlobalNode = false;
        }

        // cut off zone
        try
        {
          // find parent node
          if ( ( string.IsNullOrEmpty( tokenZone ) )
          ||   ( !zoneNodes.ContainsKey( tokenZone ) )
          ||   ( addToGlobalNode ) )
          {
            globalZone.Nodes.Add( node );
            if ( ( expandedNodes.ContainsKey( globalZone.Text ) )
            &&   ( expandedNodes[globalZone.Text] ) )
            {
              globalZone.Expand();
            }
          }
          else 
          {
            if ( !zoneNodes.ContainsKey( tokenZone ) )
            {
              var zoneNode = new System.Windows.Forms.TreeNode();

              zoneNode.Text = tokenZone;
              zoneNode.Tag  = null;
              zoneNode.ImageIndex = zoneNode.SelectedImageIndex = 0;
              NodeRoot.Nodes.Add( zoneNode );
              zoneNodes.Add( zoneNode.Text, zoneNode );
            }
            var   parentZoneNode = zoneNodes[tokenZone];

            dotPos = node.Text.IndexOf( '.' );
            string    nodeParentText = parentZoneNode.Text;
            if ( dotPos != -1 )
            {
              node.Text = node.Text.Substring( dotPos );
            }
            parentZoneNode.Nodes.Add( node );

            if ( ( expandedNodes.ContainsKey( nodeParentText ) )
            &&   ( expandedNodes[nodeParentText] ) )
            {
              parentZoneNode.Expand();
            }
          }
        }
        catch ( Exception ex )
        {
          Debug.Log( ex.Message );
        }

      }
      NodeRoot.Expand();
      treeProject.EndUpdate();
    }



    private TreeNode FindNodeByText( string Text, TreeNode ParentNode )
    {
      foreach ( System.Windows.Forms.TreeNode node in NodeRoot.Nodes )
      {
        if ( node.Text == Text )
        {
          return node;
        }
      }
      return null;
    }



    private void checkShowLocalLabels_Click( object sender, EventArgs e )
    {
      Core.Settings.LabelExplorerShowLocalLabels = !Core.Settings.LabelExplorerShowLocalLabels;

      checkShowLocalLabels.Image = Core.Settings.LabelExplorerShowLocalLabels ? RetroDevStudio.Properties.Resources.flag_green_on.ToBitmap() : RetroDevStudio.Properties.Resources.flag_green_off.ToBitmap();

      StoreOpenNodes();
      RefreshNodes();
    }



    private void checkShowShortCutLabels_Click( object sender, EventArgs e )
    {
      Core.Settings.LabelExplorerShowShortCutLabels = !Core.Settings.LabelExplorerShowShortCutLabels;

      checkShowShortCutLabels.Image = Core.Settings.LabelExplorerShowShortCutLabels ? RetroDevStudio.Properties.Resources.flag_blue_on.ToBitmap() : RetroDevStudio.Properties.Resources.flag_blue_off.ToBitmap();

      StoreOpenNodes();
      RefreshNodes();
    }



    private void checkSortBySource_Click( object sender, EventArgs e )
    {
      Core.Settings.LabelExplorerSortByIndex    = true;
      Core.Settings.LabelExplorerSortByAlphabet = false;

      checkSortAlphabetically.Enabled = true;
      checkSortBySource.Enabled       = false;

      StoreOpenNodes();
      RefreshNodes();
    }



    private void checkSortAlphabetically_Click( object sender, EventArgs e )
    {
      Core.Settings.LabelExplorerSortByIndex = false;
      Core.Settings.LabelExplorerSortByAlphabet = true;

      checkSortBySource.Enabled = true;
      checkSortAlphabetically.Enabled = false;

      StoreOpenNodes();
      RefreshNodes();
    }



    private void editOutlineFilter_TextChanged( object sender, EventArgs e )
    {
      Core.Settings.LabelExplorerFilter = editLabelExplorerFilter.Text;
      StoreOpenNodes();
      RefreshNodes();
    }



    private void treeProject_AfterSelect( object sender, TreeViewEventArgs e )
    {
      SymbolInfo tokenInfo = (SymbolInfo)e.Node.Tag;
      if ( tokenInfo == null )
      {
        return;
      }

      var doc = Core.Navigating.FindDocumentByPath( tokenInfo.DocumentFilename );
      if ( doc != null )
      {
        doc.HighlightText( tokenInfo.LocalLineIndex, tokenInfo.CharIndex, tokenInfo.Length );
      }

    }



  }
}
