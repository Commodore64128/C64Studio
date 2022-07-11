﻿using RetroDevStudio.Documents;



namespace RetroDevStudio.Tasks
{
  public class TaskRefreshOutline : Task
  {
    private BaseDocument    m_Document;



    public TaskRefreshOutline( BaseDocument Document )
    {
      m_Document = Document;
    }



    protected override bool ProcessTask()
    {
      Core.MainForm.m_Outline.RefreshFromDocument( m_Document );
      return true;
    }
  }
}
