﻿using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using System;
using System.Collections.Generic;
using MediaBrowser.Model.Serialization;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.IO;
using System.Linq;

namespace MediaBrowser.Controller.LiveTv
{
    public class LiveTvVideoRecording : Video, ILiveTvRecording
    {
        [IgnoreDataMember]
        public string EpisodeTitle { get; set; }
        [IgnoreDataMember]
        public bool IsSeries { get; set; }
        public string SeriesTimerId { get; set; }
        public string TimerId { get; set; }
        [IgnoreDataMember]
        public DateTime StartDate { get; set; }
        public RecordingStatus Status { get; set; }
        [IgnoreDataMember]
        public bool IsSports { get; set; }
        [IgnoreDataMember]
        public bool IsNews { get; set; }
        [IgnoreDataMember]
        public bool IsKids { get; set; }
        [IgnoreDataMember]
        public bool IsRepeat { get; set; }
        [IgnoreDataMember]
        public bool IsMovie { get; set; }
        [IgnoreDataMember]
        public bool IsLive { get; set; }
        [IgnoreDataMember]
        public bool IsPremiere { get; set; }

        [IgnoreDataMember]
        public override SourceType SourceType
        {
            get { return SourceType.LiveTV; }
        }

        [IgnoreDataMember]
        public override string MediaType
        {
            get
            {
                return Model.Entities.MediaType.Video;
            }
        }

        [IgnoreDataMember]
        public override bool SupportsPlayedStatus
        {
            get
            {
                return Status == RecordingStatus.Completed && base.SupportsPlayedStatus;
            }
        }

        [IgnoreDataMember]
        public override LocationType LocationType
        {
            get
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    return base.LocationType;
                }

                return LocationType.Remote;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is owned item.
        /// </summary>
        /// <value><c>true</c> if this instance is owned item; otherwise, <c>false</c>.</value>
        [IgnoreDataMember]
        public override bool IsOwnedItem
        {
            get
            {
                return false;
            }
        }

        public override string GetClientTypeName()
        {
            return "Recording";
        }

        public override bool IsSaveLocalMetadataEnabled()
        {
            return false;
        }

        public override double? GetDefaultPrimaryImageAspectRatio()
        {
            return LiveTvProgram.GetDefaultPrimaryImageAspectRatio(this);
        }

        [IgnoreDataMember]
        public override bool SupportsLocalMetadata
        {
            get
            {
                return false;
            }
        }

        public override UnratedItem GetBlockUnratedType()
        {
            return UnratedItem.LiveTvProgram;
        }

        protected override string GetInternalMetadataPath(string basePath)
        {
            return System.IO.Path.Combine(basePath, "livetv", Id.ToString("N"));
        }

        public override bool CanDelete()
        {
            if (string.Equals(ServiceName, "Emby", StringComparison.OrdinalIgnoreCase))
            {
                return Status == RecordingStatus.Completed;
            }
            return true;
        }

        public override bool IsAuthorizedToDelete(User user, List<Folder> allCollectionFolders)
        {
            return user.Policy.EnableLiveTvManagement;
        }

        public override List<MediaSourceInfo> GetMediaSources(bool enablePathSubstitution)
        {
            var list = base.GetMediaSources(enablePathSubstitution);

            foreach (var mediaSource in list)
            {
                if (string.IsNullOrEmpty(mediaSource.Path))
                {
                    mediaSource.Type = MediaSourceType.Placeholder;
                }
            }

            return list;
        }

        public override bool IsVisibleStandalone(User user)
        {
            return IsVisible(user);
        }

        public override IEnumerable<FileSystemMetadata> GetDeletePaths()
        {
            return new[] {
                new FileSystemMetadata
                {
                    FullName = Path,
                    IsDirectory = IsFolder
                }
            }.Concat(GetLocalMetadataFilesToDelete());
        }
    }
}
