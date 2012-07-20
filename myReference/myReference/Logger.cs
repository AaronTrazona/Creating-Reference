#define Logger
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace myReference
{
#if Logger
    #region Class Logger
    
    /// <summary>
    /// this class can't be inherit.
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// Instantiate the Message Type in Hashtable
        /// </summary>
        Hashtable visibleDebug = new Hashtable();
        Hashtable visibleWarning = new Hashtable();
        Hashtable visibleError = new Hashtable();
        Hashtable visibleInfo = new Hashtable();

        private bool visibleDebugDef = true;
        private bool visibleErrorDef = true;
        private bool visibleInfoDef = true;
        private bool visibleWarningDef = true;

        private static Logger logger;
        private string logfilepath;

        /// <summary>
        /// Getters and Setters for LogFilePath
        /// </summary>
        public string LogFilePath 
        {
            get { return logfilepath; }
            set { logfilepath = value;}
        }
        
        /// <summary>
        /// Instantiate the Class Logger
        /// </summary>
        /// <returns></returns>
        public static Logger Instance() 
        {

            if (logger == null)
                logger = new Logger(); 
            return logger;
        }

        /// <summary>
        /// Messages Type  
        /// </summary>
        public enum MessageLevel 
        { 
            Info,
            Warning,
            Error,
            Debug
        }

        /// <summary>
        /// Get the Project and Method Name.
        /// </summary>
        /// <returns></returns>
        public string DebugInfo() 
        {
            System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(2, true);
            System.Reflection.MethodBase sb = sf.GetMethod();
            return string.Concat(sb.DeclaringType.Name, ":", sb.Name);
        }

        /// <summary>
        /// Send the Message and LogFilePath to the Stream Writer 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        private void writeLine(string type, string msg)
        { 
            System.Diagnostics.Debug.Print(string.Format("{0}[{1}]{2}",type,Thread.CurrentThread.Name,msg));
            string date = string.Format(@"{0:MM-dd-yyyy}",DateTime.Now);
            Utility.writeToLogFile(string.Format("{0}[{1}] {2}",type,Thread.CurrentThread.Name,msg),string.Format(logfilepath,date));
        }

        /// <summary>
        /// Send the Message and LogFilePath to the Stream Writer 
        /// </summary>
        /// <param name="msg"></param>
        private void writeLine(string msg) 
        {
            System.Diagnostics.Debug.Print(msg);
            string date = string.Format(@"{0:MM-dd-yyyy}",DateTime.Now);
            Utility.writeToLogFile(msg,string.Format(logfilepath,date));
        }

        /// <summary>
        /// Checking if the Hashtable contains a Key.
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool isVisible(MessageLevel messageLevel, int hash) 
        {
            switch (messageLevel)
            { 
                case MessageLevel.Debug:
                    return visibleDebug.ContainsKey(hash)? (bool)visibleDebug[hash] : visibleDebugDef;
                case MessageLevel.Error:
                    return visibleError.ContainsKey(hash) ? (bool)visibleError[hash] : visibleErrorDef;
                case MessageLevel.Info:
                    return visibleInfo.ContainsKey(hash) ? (bool)visibleInfo[hash] : visibleInfoDef;
                case MessageLevel.Warning:
                    return visibleWarning.ContainsKey(hash) ? (bool)visibleWarning[hash] : visibleWarningDef;
                
            }
            return true;
        }

        /// <summary>
        /// Overload for Debug Message
        /// </summary>
        /// <param name="o"></param>
        #region Debug

        public void Debug(object o)
        { 
            string caller = DebugInfo();
            if(isVisible(MessageLevel.Debug,caller.GetHashCode()))
                writeLine("  Debug: ",string.Concat(caller," - ",o.ToString()));    
        }

        public void Debug(string msg)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Debug,caller.GetHashCode()))
                writeLine("  Debug: ",string.Concat(caller," - ",msg));
        }

        public void Debug(string format, params object[] args)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Debug,caller.GetHashCode()))
                writeLine("  Debug: ",string.Concat(caller," - ",string.Format(format,args)));
        }

        public void Debug(string msg, Exception ex)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Debug, caller.GetHashCode()))
                writeLine("  Debug: ", string.Concat(caller, " - ", string.Format("{0}\n{1}", msg, ex)));
        }
        
        #endregion

        /// <summary>
        /// Overload for Warning Message
        /// </summary>
        /// <param name="o"></param>
        #region Warning

        public void Warning(object o)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Warning,caller.GetHashCode()))
                writeLine("  Warning: ",string.Concat(caller," - ", o.ToString()));
        }

        public void Warning(string msg)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Warning,caller.GetHashCode()))
                writeLine("  Warning: ",string.Concat(caller," - ",msg));
        }

        public void Warning(string format, params object[] args)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Warning, caller.GetHashCode()))
                writeLine("  Warning: ", string.Concat(caller, " - ", string.Format(format, args)));
        }

        public void Warning(string msg, Exception ex)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Warning,caller.GetHashCode()))
                writeLine("  Warning: ",string.Concat(caller," - ",string.Format("{0}\n{1}",msg,ex)));
        }
        
        #endregion

        /// <summary>
        /// Overload for Info Message
        /// </summary>
        /// <param name="msg"></param>
        #region Info
      
        public void Info(object o)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Info, caller.GetHashCode()))
                writeLine(" Info: ", string.Concat(caller, " - ", o.ToString()));
        }

        public void Info(string msg)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Info, caller.GetHashCode()))
                writeLine("  Info: ",string.Concat(caller," - ",msg));
        }

        public void Info(string format, params object[] args)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Info,caller.GetHashCode()))
                writeLine("  Info: ",string.Concat(caller," - ",string.Format(format,args)));
	    }

        public void Info(string msg, Exception ex)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Info, caller.GetHashCode()))
                writeLine("  Info: ", string.Concat(caller, " - ", string.Format("{0}\n{1}", msg, ex)));
        }

        #endregion

        /// <summary>
        /// Overload for Error Message
        /// </summary>
        /// <param name="o"></param>
        #region Error

        public void Error(object o)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Error, caller.GetHashCode()))
                writeLine("  Error: ", string.Concat(caller, " - ", o.ToString()));
        }

        public void Error(string msg)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Error,caller.GetHashCode()))
                writeLine("  Error: ",string.Concat(caller," - ",msg));
        }

        public void Error(string format, params object[] args)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Error, caller.GetHashCode()))
                writeLine(" Error: ", string.Format(format, args));
        }

        public void Error(string msg, Exception ex)
        {
            string caller = DebugInfo();
            if (isVisible(MessageLevel.Error, caller.GetHashCode()))
                writeLine("  Error: ", string.Format("{0}\n{1}", msg, ex));
        }

        #endregion
    } 

    #endregion 
#endif
}
