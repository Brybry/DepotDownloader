using SteamKit2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DepotDownloader
{
    // Hack to add current and new code to SteamKit2 because I'm too lazy to rebuild it
    // TODO: add to SteamKit2 itself and then bump version requirement
    public static class SteamAppsExtensions
    {
        #region WorkshopItemInfoRequest/Response hack
        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CMsgClientWorkshopItemInfoRequest")]
        public partial class CMsgClientWorkshopItemInfoRequest : global::ProtoBuf.IExtensible
        {
            public CMsgClientWorkshopItemInfoRequest() {}
            

            private uint _app_id = default(uint);
            [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"app_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue(default(uint))]
            public uint app_id
            {
            get { return _app_id; }
            set { _app_id = value; }
            }

            private uint _last_time_updated = default(uint);
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"last_time_updated", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue(default(uint))]
            public uint last_time_updated
            {
            get { return _last_time_updated; }
            set { _last_time_updated = value; }
            }
            private readonly global::System.Collections.Generic.List<CMsgClientWorkshopItemInfoRequest.WorkshopItem> _workshop_items = new global::System.Collections.Generic.List<CMsgClientWorkshopItemInfoRequest.WorkshopItem>();
            [global::ProtoBuf.ProtoMember(3, Name=@"workshop_items", DataFormat = global::ProtoBuf.DataFormat.Default)]
            public global::System.Collections.Generic.List<CMsgClientWorkshopItemInfoRequest.WorkshopItem> workshop_items
            {
            get { return _workshop_items; }
            }
        
        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"WorkshopItem")]
        public partial class WorkshopItem : global::ProtoBuf.IExtensible
        {
            public WorkshopItem() {}
            

            private ulong _published_file_id = default(ulong);
            [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"published_file_id", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(ulong))]
            public ulong published_file_id
            {
            get { return _published_file_id; }
            set { _published_file_id = value; }
            }

            private uint _time_updated = default(uint);
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"time_updated", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue(default(uint))]
            public uint time_updated
            {
            get { return _time_updated; }
            set { _time_updated = value; }
            }
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }
        
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }
        
        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CMsgClientWorkshopItemInfoResponse")]
        public partial class CMsgClientWorkshopItemInfoResponse : global::ProtoBuf.IExtensible
        {
            public CMsgClientWorkshopItemInfoResponse() {}
            

            private int _eresult = (int)2;
            [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"eresult", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue((int)2)]
            public int eresult
            {
            get { return _eresult; }
            set { _eresult = value; }
            }

            private uint _update_time = default(uint);
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"update_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue(default(uint))]
            public uint update_time
            {
            get { return _update_time; }
            set { _update_time = value; }
            }
            private readonly global::System.Collections.Generic.List<CMsgClientWorkshopItemInfoResponse.WorkshopItemInfo> _workshop_items = new global::System.Collections.Generic.List<CMsgClientWorkshopItemInfoResponse.WorkshopItemInfo>();
            [global::ProtoBuf.ProtoMember(3, Name=@"workshop_items", DataFormat = global::ProtoBuf.DataFormat.Default)]
            public global::System.Collections.Generic.List<CMsgClientWorkshopItemInfoResponse.WorkshopItemInfo> workshop_items
            {
            get { return _workshop_items; }
            }
        
            private readonly global::System.Collections.Generic.List<ulong> _private_items = new global::System.Collections.Generic.List<ulong>();
            [global::ProtoBuf.ProtoMember(4, Name=@"private_items", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            public global::System.Collections.Generic.List<ulong> private_items
            {
            get { return _private_items; }
            }
        
        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"WorkshopItemInfo")]
        public partial class WorkshopItemInfo : global::ProtoBuf.IExtensible
        {
            public WorkshopItemInfo() {}
            

            private ulong _published_file_id = default(ulong);
            [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"published_file_id", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(ulong))]
            public ulong published_file_id
            {
            get { return _published_file_id; }
            set { _published_file_id = value; }
            }

            private uint _time_updated = default(uint);
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"time_updated", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue(default(uint))]
            public uint time_updated
            {
            get { return _time_updated; }
            set { _time_updated = value; }
            }

            private ulong _manifest_id = default(ulong);
            [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"manifest_id", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(ulong))]
            public ulong manifest_id
            {
            get { return _manifest_id; }
            set { _manifest_id = value; }
            }

            private bool _is_legacy = default(bool);
            [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"is_legacy", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(default(bool))]
            public bool is_legacy
            {
            get { return _is_legacy; }
            set { _is_legacy = value; }
            }
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }
        
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }        
        #endregion

        #region New Handler         
        public class DepotHandler : ClientMsgHandler
        {
            // TODO: Use ReadOnlyCollection instead of List?
            public class WorkshopItemInfoRequestCallback : CallbackMsg
            {
                /// <summary>
                /// Gets the result of requesting workshop item's info 
                /// </summary>
                public EResult Result { get; private set; }
                
                /// <summary>
                /// Gets ???
                /// </summary>
                public uint UpdateTime { get; private set; }

                /// <summary>
                /// Gets the actual workshop item info
                /// </summary>
                public List<CMsgClientWorkshopItemInfoResponse.WorkshopItemInfo> ItemInfo { get; private set; }

                /// <summary>
                /// Gets ???
                /// </summary>
                public List<ulong> PrivateItems  { get; private set; }
                
                internal WorkshopItemInfoRequestCallback( JobID jobID, CMsgClientWorkshopItemInfoResponse msg )
                {
                    JobID = jobID;
                    Result = ( EResult )msg.eresult;
                    UpdateTime = msg.update_time;
                    ItemInfo = msg.workshop_items;
                    PrivateItems = msg.private_items;                
                }
            }
            
            public JobID GetWorkshopItemInfo(uint appid, ulong itemid)
            {
                // var request = new ClientMsgProtobuf<CMsgClientWorkshopItemInfoRequest>( EMsg.ClientWorkshopItemInfoRequest );
                var request = new ClientMsgProtobuf<CMsgClientWorkshopItemInfoRequest>( (EMsg)7384 );
               
                request.SourceJobID = Client.GetNextJobID();
                request.Body.app_id = appid;
                var item = new CMsgClientWorkshopItemInfoRequest.WorkshopItem();
                item.published_file_id = itemid;
                request.Body.workshop_items.Add(item);
                
                Client.Send(request);
                return request.SourceJobID;
            }
            
            public override void HandleMsg( IPacketMsg packetMsg )
            {
                switch ( packetMsg.MsgType )
                {

                    //case EMsg.ClientWorkshopItemInfoResponse:
                    case (EMsg)7385:
                        HandleWorkshopItemInfoResponse( packetMsg );
                        break;

                }            
            }

            void HandleWorkshopItemInfoResponse( IPacketMsg packetMsg )
            {
                var response = new ClientMsgProtobuf<CMsgClientWorkshopItemInfoResponse>( packetMsg );
                var callback = new WorkshopItemInfoRequestCallback(response.TargetJobID, response.Body);
                this.Client.PostCallback( callback );
            }
        
        }
        #endregion
    }

    class Steam3Session
    {
        public class Credentials
        {
            public bool LoggedOn { get; set; }
            public ulong SessionToken { get; set; }

            public bool IsValid
            {
                get { return LoggedOn; }
            }
        }

        public ReadOnlyCollection<SteamApps.LicenseListCallback.License> Licenses
        {
            get;
            private set;
        }

        public Dictionary<uint, byte[]> AppTickets { get; private set; }
        public Dictionary<uint, ulong> AppTokens { get; private set; }
        public Dictionary<uint, byte[]> DepotKeys { get; private set; }
        // TODO: fix this lol
        public Dictionary<ulong, SteamAppsExtensions.CMsgClientWorkshopItemInfoResponse.WorkshopItemInfo> WorkshopItems { get; private set; }
        public Dictionary<Tuple<uint, string>, SteamApps.CDNAuthTokenCallback> CDNAuthTokens { get; private set; }
        public Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo> AppInfo { get; private set; }
        public Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo> PackageInfo { get; private set; }

        public SteamClient steamClient;
        public SteamUser steamUser;
        SteamApps steamApps;
        public SteamAppsExtensions.DepotHandler steamDepot;

        CallbackManager callbacks;

        bool authenticatedUser;
        bool bConnected;
        bool bConnecting;
        bool bAborted;
        int seq; // more hack fixes
        DateTime connectTime;

        // input
        SteamUser.LogOnDetails logonDetails;

        // output
        Credentials credentials;

        static readonly TimeSpan STEAM3_TIMEOUT = TimeSpan.FromSeconds( 30 );


        public Steam3Session( SteamUser.LogOnDetails details )
        {
            this.logonDetails = details;

            this.authenticatedUser = details.Username != null;
            this.credentials = new Credentials();
            this.bConnected = false;
            this.bConnecting = false;
            this.bAborted = false;
            this.seq = 0;

            this.AppTickets = new Dictionary<uint, byte[]>();
            this.AppTokens = new Dictionary<uint, ulong>();
            this.DepotKeys = new Dictionary<uint, byte[]>();
            this.WorkshopItems = new Dictionary<ulong, SteamAppsExtensions.CMsgClientWorkshopItemInfoResponse.WorkshopItemInfo>();
            this.CDNAuthTokens = new Dictionary<Tuple<uint, string>, SteamApps.CDNAuthTokenCallback>();
            this.AppInfo = new Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo>();
            this.PackageInfo = new Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo>();

            this.steamClient = new SteamClient();
            
            this.steamClient.AddHandler( new SteamAppsExtensions.DepotHandler() );

            this.steamUser = this.steamClient.GetHandler<SteamUser>();
            this.steamApps = this.steamClient.GetHandler<SteamApps>();
            this.steamDepot = this.steamClient.GetHandler<SteamAppsExtensions.DepotHandler>();

            this.callbacks = new CallbackManager(this.steamClient);

            this.callbacks.Subscribe<SteamClient.ConnectedCallback>(ConnectedCallback);
            this.callbacks.Subscribe<SteamClient.DisconnectedCallback>(DisconnectedCallback);
            this.callbacks.Subscribe<SteamUser.LoggedOnCallback>(LogOnCallback);
            this.callbacks.Subscribe<SteamUser.SessionTokenCallback>(SessionTokenCallback);
            this.callbacks.Subscribe<SteamApps.LicenseListCallback>(LicenseListCallback);
            this.callbacks.Subscribe<SteamUser.UpdateMachineAuthCallback>(UpdateMachineAuthCallback);

            Console.Write( "Connecting to Steam3..." );

            if ( authenticatedUser )
            {
                FileInfo fi = new FileInfo(String.Format("{0}.sentryFile", logonDetails.Username));
                if (ConfigStore.TheConfig.SentryData != null && ConfigStore.TheConfig.SentryData.ContainsKey(logonDetails.Username))
                {
                    logonDetails.SentryFileHash = Util.SHAHash(ConfigStore.TheConfig.SentryData[logonDetails.Username]);
                }
                else if (fi.Exists && fi.Length > 0)
                {
                    var sentryData = File.ReadAllBytes(fi.FullName);
                    logonDetails.SentryFileHash = Util.SHAHash(sentryData);
                    ConfigStore.TheConfig.SentryData[logonDetails.Username] = sentryData;
                    ConfigStore.Save();
                }
            }

            Connect();
        }

        public delegate bool WaitCondition();
        public bool WaitUntilCallback(Action submitter, WaitCondition waiter)
        {
            while (!bAborted && !waiter())
            {
                submitter();

                int seq = this.seq;
                do
                {
                    WaitForCallbacks();
                }
                while (!bAborted && this.seq == seq && !waiter());
            }

            return bAborted;
        }

        public Credentials WaitForCredentials()
        {
            if (credentials.IsValid || bAborted)
                return credentials;

            WaitUntilCallback(() => { }, () => { return credentials.IsValid; });

            return credentials;
        }

        public void RequestAppInfo(uint appId)
        {
            if (AppInfo.ContainsKey(appId) || bAborted)
                return;

            bool completed = false;
            Action<SteamApps.PICSTokensCallback> cbMethodTokens = (appTokens) =>
            {
                completed = true;
                if (appTokens.AppTokensDenied.Contains(appId))
                {
                    Console.WriteLine("Insufficient privileges to get access token for app {0}", appId);
                }

                foreach (var token_dict in appTokens.AppTokens)
                {
                    this.AppTokens.Add(token_dict.Key, token_dict.Value);
                }
            };

            WaitUntilCallback(() => { 
                callbacks.Subscribe(steamApps.PICSGetAccessTokens(new List<uint>() { appId }, new List<uint>() { }), cbMethodTokens);
            }, () => { return completed; });

            completed = false;
            Action<SteamApps.PICSProductInfoCallback> cbMethod = (appInfo) =>
            {
                completed = !appInfo.ResponsePending;

                foreach (var app_value in appInfo.Apps)
                {
                    var app = app_value.Value;

                    Console.WriteLine("Got AppInfo for {0}", app.ID);
                    AppInfo.Add(app.ID, app);
                }

                foreach (var app in appInfo.UnknownApps)
                {
                    AppInfo.Add(app, null);
                }
            };

            SteamApps.PICSRequest request = new SteamApps.PICSRequest(appId);
            if (AppTokens.ContainsKey(appId))
            {
                request.AccessToken = AppTokens[appId];
                request.Public = false;
            }

            WaitUntilCallback(() => {
                callbacks.Subscribe(steamApps.PICSGetProductInfo(new List<SteamApps.PICSRequest>() { request }, new List<SteamApps.PICSRequest>() { }), cbMethod);
            }, () => { return completed; });
        }

        public void RequestPackageInfo(IEnumerable<uint> packageIds)
        {
            List<uint> packages = packageIds.ToList();
            packages.RemoveAll(pid => PackageInfo.ContainsKey(pid));

            if (packages.Count == 0 || bAborted)
                return;

            bool completed = false;
            Action<SteamApps.PICSProductInfoCallback> cbMethod = (packageInfo) =>
            {
                completed = !packageInfo.ResponsePending;

                foreach (var package_value in packageInfo.Packages)
                {
                    var package = package_value.Value;
                    PackageInfo.Add(package.ID, package);
                }

                foreach (var package in packageInfo.UnknownPackages)
                {
                    PackageInfo.Add(package, null);
                }
            };

            WaitUntilCallback(() => {
                callbacks.Subscribe(steamApps.PICSGetProductInfo(new List<uint>(), packages), cbMethod);
            }, () => { return completed; });
        }

        public void RequestAppTicket(uint appId)
        {
            if (AppTickets.ContainsKey(appId) || bAborted)
                return;


            if ( !authenticatedUser )
            {
                AppTickets[appId] = null;
                return;
            }

            bool completed = false;
            Action<SteamApps.AppOwnershipTicketCallback> cbMethod = (appTicket) =>
            {
                completed = true;

                if (appTicket.Result != EResult.OK)
                {
                    Console.WriteLine("Unable to get appticket for {0}: {1}", appTicket.AppID, appTicket.Result);
                    Abort();
                }
                else
                {
                    Console.WriteLine("Got appticket for {0}!", appTicket.AppID);
                    AppTickets[appTicket.AppID] = appTicket.Ticket;
                }
            };

            WaitUntilCallback(() => { 
                callbacks.Subscribe(steamApps.GetAppOwnershipTicket(appId), cbMethod);
            }, () => { return completed; });
        }

        public void RequestDepotKey(uint depotId, uint appid = 0)
        {
            if (DepotKeys.ContainsKey(depotId) || bAborted)
                return;

            bool completed = false;

            Action<SteamApps.DepotKeyCallback> cbMethod = (depotKey) =>
            {
                completed = true;
                Console.WriteLine("Got depot key for {0} result: {1}", depotKey.DepotID, depotKey.Result);

                if (depotKey.Result != EResult.OK)
                {
                    Abort();
                    return;
                }

                DepotKeys[depotKey.DepotID] = depotKey.DepotKey;
            };

            WaitUntilCallback(() =>
            {
                callbacks.Subscribe(steamApps.GetDepotDecryptionKey(depotId, appid), cbMethod);
            }, () => { return completed; });
        }

        public void RequestCDNAuthToken(uint depotid, string host)
        {
            if (CDNAuthTokens.ContainsKey(Tuple.Create(depotid, host)) || bAborted)
                return;

            bool completed = false;
            Action<SteamApps.CDNAuthTokenCallback> cbMethod = (cdnAuth) =>
            {
                completed = true;
                Console.WriteLine("Got CDN auth token for {0} result: {1}", host, cdnAuth.Result);

                if (cdnAuth.Result != EResult.OK)
                {
                    Abort();
                    return;
                }

                CDNAuthTokens[Tuple.Create(depotid, host)] = cdnAuth;
            };

            WaitUntilCallback(() =>
            {
                callbacks.Subscribe(steamApps.GetCDNAuthToken(depotid, host), cbMethod);
            }, () => { return completed; });
        }

        public void GetWorkshopItemInfo(uint appid, uint itemid)
        {
            bool completed = false;
            Action<SteamAppsExtensions.DepotHandler.WorkshopItemInfoRequestCallback> cbMethod = (response) =>
            {
                completed = true;
                Console.WriteLine("Got workshop item info.");
                if (response.Result != EResult.OK)
                {
                    Abort();
                    return;
                }
 
                foreach (var itemInfo in response.ItemInfo)
                {
                    this.WorkshopItems[itemInfo.published_file_id] = itemInfo;
                }
            };
            
            WaitUntilCallback(() =>
            {
                callbacks.Subscribe(steamDepot.GetWorkshopItemInfo(appid, itemid), cbMethod);
            }, () => { return completed; });
        }
        
        void Connect()
        {
            bAborted = false;
            bConnected = false;
            bConnecting = true;
            this.connectTime = DateTime.Now;
            this.steamClient.Connect();
        }

        private void Abort(bool sendLogOff=true)
        {
            Disconnect(sendLogOff);
        }
        public void Disconnect(bool sendLogOff=true)
        {
            if (sendLogOff)
            {
                steamUser.LogOff();
            }
            
            steamClient.Disconnect();
            bConnected = false;
            bConnecting = false;
            bAborted = true;

            // flush callbacks
            callbacks.RunCallbacks();
        }


        private void WaitForCallbacks()
        {
            callbacks.RunWaitCallbacks( TimeSpan.FromSeconds(1) );

            TimeSpan diff = DateTime.Now - connectTime;

            if (diff > STEAM3_TIMEOUT && !bConnected)
            {
                Console.WriteLine("Timeout connecting to Steam3.");
                Abort();

                return;
            }
        }

        private void ConnectedCallback(SteamClient.ConnectedCallback connected)
        {
            Console.WriteLine(" Done!");
            bConnecting = false;
            bConnected = true;
            if ( !authenticatedUser )
            {
                Console.Write( "Logging anonymously into Steam3..." );
                steamUser.LogOnAnonymous();
            }
            else
            {
                Console.Write( "Logging '{0}' into Steam3...", logonDetails.Username );
                steamUser.LogOn( logonDetails );
            }
        }

        private void DisconnectedCallback(SteamClient.DisconnectedCallback disconnected)
        {
            if ((!bConnected && !bConnecting) || bAborted)
                return;

            Console.WriteLine("Reconnecting");
            steamClient.Connect();
        }

        private void LogOnCallback(SteamUser.LoggedOnCallback loggedOn)
        {
            bool isSteamGuard = loggedOn.Result == EResult.AccountLogonDenied;
            bool is2FA = loggedOn.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            if (isSteamGuard || is2FA)
            {
                Console.WriteLine("This account is protected by Steam Guard.");

                Abort(false);

                if (is2FA)
                {
                    Console.Write("Please enter your 2 factor auth code from your authenticator app: ");
                    logonDetails.TwoFactorCode = Console.ReadLine();
                }
                else
                {
                    Console.Write("Please enter the authentication code sent to your email address: ");
                    logonDetails.AuthCode = Console.ReadLine();
                }

                Console.Write("Retrying Steam3 connection...");
                Connect();

                return;
            }
            else if (loggedOn.Result == EResult.ServiceUnavailable)
            {
                Console.WriteLine("Unable to login to Steam3: {0}", loggedOn.Result);
                Abort(false);

                return;
            }
            else if (loggedOn.Result != EResult.OK)
            {
                Console.WriteLine("Unable to login to Steam3: {0}", loggedOn.Result);
                Abort();
                
                return;
            }

            Console.WriteLine(" Done!");

            this.seq++;
            credentials.LoggedOn = true;

            if (ContentDownloader.Config.CellID == 0)
            {
                Console.WriteLine("Using Steam3 suggested CellID: " + loggedOn.CellID);
                ContentDownloader.Config.CellID = (int)loggedOn.CellID;
            }
        }

        private void SessionTokenCallback(SteamUser.SessionTokenCallback sessionToken)
        {
            Console.WriteLine("Got session token!");
            credentials.SessionToken = sessionToken.SessionToken;
        }

        private void LicenseListCallback(SteamApps.LicenseListCallback licenseList)
        {
            if (licenseList.Result != EResult.OK)
            {
                Console.WriteLine("Unable to get license list: {0} ", licenseList.Result);
                Abort();

                return;
            }

            Console.WriteLine("Got {0} licenses for account!", licenseList.LicenseList.Count);
            Licenses = licenseList.LicenseList;

            IEnumerable<uint> licenseQuery = Licenses.Select(lic =>
            {
                return lic.PackageID;
            });

            Console.WriteLine("Licenses: {0}", string.Join(", ", licenseQuery));
        }

        private void UpdateMachineAuthCallback(SteamUser.UpdateMachineAuthCallback machineAuth)
        {
            byte[] hash = Util.SHAHash(machineAuth.Data);
            Console.WriteLine("Got Machine Auth: {0} {1} {2} {3}", machineAuth.FileName, machineAuth.Offset, machineAuth.BytesToWrite, machineAuth.Data.Length, hash);

            ConfigStore.TheConfig.SentryData[logonDetails.Username] = machineAuth.Data;
            ConfigStore.Save();
            
            var authResponse = new SteamUser.MachineAuthDetails
            {
                BytesWritten = machineAuth.BytesToWrite,
                FileName = machineAuth.FileName,
                FileSize = machineAuth.BytesToWrite,
                Offset = machineAuth.Offset,

                SentryFileHash = hash, // should be the sha1 hash of the sentry file we just wrote

                OneTimePassword = machineAuth.OneTimePassword, // not sure on this one yet, since we've had no examples of steam using OTPs

                LastError = 0, // result from win32 GetLastError
                Result = EResult.OK, // if everything went okay, otherwise ~who knows~

                JobID = machineAuth.JobID, // so we respond to the correct server job
            };

            // send off our response
            steamUser.SendMachineAuthResponse( authResponse );
        }


    }
}
