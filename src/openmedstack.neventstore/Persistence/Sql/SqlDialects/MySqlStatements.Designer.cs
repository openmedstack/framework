﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenMedStack.NEventStore.Persistence.Sql.SqlDialects {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MySqlStatements {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MySqlStatements() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OpenMedStack.NEventStore.Persistence.Sql.SqlDialects.MySqlStatements", typeof(MySqlStatements).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE IF NOT EXISTS Commits
        ///(
        ///    BucketId varchar(40) charset utf8 NOT NULL,
        ///    StreamId varchar(40) charset utf8 NOT NULL,
        ///    StreamIdOriginal varchar(1000) charset utf8 NOT NULL,
        ///    StreamRevision int NOT NULL CHECK (StreamRevision &gt; 0),
        ///    Items tinyint NOT NULL CHECK (Items &gt; 0),
        ///    CommitId binary(16) NOT NULL CHECK (CommitId != 0),
        ///    CommitSequence int NOT NULL CHECK (CommitSequence &gt; 0),
        ///    CommitStamp bigint NOT NULL,
        ///    CheckpointNumber bigint AUTO_INCREMENT,
        ///    Headers [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string InitializeStorage {
            get {
                return ResourceManager.GetString("InitializeStorage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT
        ///  INTO Commits
        ///     ( BucketId, StreamId, StreamIdOriginal, CommitId, CommitSequence, StreamRevision, Items, CommitStamp, Headers, Payload )
        ///VALUES (@BucketId, @StreamId, @StreamIdOriginal, @CommitId, @CommitSequence, @StreamRevision, @Items, @CommitStamp, @Headers, @Payload);
        ///SELECT LAST_INSERT_ID();.
        /// </summary>
        internal static string PersistCommit {
            get {
                return ResourceManager.GetString("PersistCommit", resourceCulture);
            }
        }
    }
}
