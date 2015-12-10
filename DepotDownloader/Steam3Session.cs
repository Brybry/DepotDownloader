using SteamKit2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DepotDownloader
{
    // Hack to add current and new code to SteamKit2 because I'm too lazy to rebuild it
    // TODO: add to SteamKit2 itself and then bump version requirement
    public static class SteamAppsExtensions
    {
        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CMsgClientCheckAppBetaPassword")]
        public partial class CMsgClientCheckAppBetaPassword : global::ProtoBuf.IExtensible
        {
            public CMsgClientCheckAppBetaPassword() {}
            
        
            private uint _app_id = default(uint);
            [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"app_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue(default(uint))]
            public uint app_id
            {
            get { return _app_id; }
            set { _app_id = value; }
            }
        
            private string _betapassword = "";
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"betapassword", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue("")]
            public string betapassword
            {
            get { return _betapassword; }
            set { _betapassword = value; }
            }
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }
        
        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CMsgClientCheckAppBetaPasswordResponse")]
        public partial class CMsgClientCheckAppBetaPasswordResponse : global::ProtoBuf.IExtensible
        {
            public CMsgClientCheckAppBetaPasswordResponse() {}
            
        
            private int _eresult = (int)2;
            [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"eresult", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            [global::System.ComponentModel.DefaultValue((int)2)]
            public int eresult
            {
            get { return _eresult; }
            set { _eresult = value; }
            }
            private readonly global::System.Collections.Generic.List<CMsgClientCheckAppBetaPasswordResponse.BetaPassword> _betapasswords = new global::System.Collections.Generic.List<CMsgClientCheckAppBetaPasswordResponse.BetaPassword>();
            [global::ProtoBuf.ProtoMember(4, Name=@"betapasswords", DataFormat = global::ProtoBuf.DataFormat.Default)]
            public global::System.Collections.Generic.List<CMsgClientCheckAppBetaPasswordResponse.BetaPassword> betapasswords
            {
            get { return _betapasswords; }
            }
        
            [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BetaPassword")]
            public partial class BetaPassword : global::ProtoBuf.IExtensible
            {
                public BetaPassword() {}
                
            
                private string _betaname = "";
                [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"betaname", DataFormat = global::ProtoBuf.DataFormat.Default)]
                [global::System.ComponentModel.DefaultValue("")]
                public string betaname
                {
                get { return _betaname; }
                set { _betaname = value; }
                }
            
                private string _betapassword = "";
                [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"betapassword", DataFormat = global::ProtoBuf.DataFormat.Default)]
                [global::System.ComponentModel.DefaultValue("")]
                public string betapassword
                {
                get { return _betapassword; }
                set { _betapassword = value; }
                }
                private global::ProtoBuf.IExtension extensionObject;
                global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
            }
        
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }
                
        public sealed class BetaPasswordCallback : CallbackMsg
        {
            /// <summary>
            /// Gets the result of requesting this beta branch + password
            /// </summary>
            public EResult Result { get; private set; }

            /// <summary>
            /// Gets the beta branch + password for this appid/password
            /// </summary>
            public List<CMsgClientCheckAppBetaPasswordResponse.BetaPassword> Password { get; private set; }


            internal BetaPasswordCallback( JobID jobID, CMsgClientCheckAppBetaPasswordResponse msg )
            {
                JobID = jobID;

                Result = ( EResult )msg.eresult;
                Password = msg.betapasswords;
            }
        }

        public static T GetPrivateField<T>(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            FieldInfo field = type.GetField(name, flags);
            return (T)field.GetValue(obj);
        }
        
        public static T GetPrivateProperty<T>(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            PropertyInfo field = type.GetProperty(name, flags);
            return (T)field.GetValue(obj, null);
        }
              
        public static JobID GetBetaPassword(this SteamApps app, uint appid, string betapassword)
        {
            var request = new ClientMsgProtobuf<CMsgClientCheckAppBetaPassword>( (EMsg)5450 );
            SteamClient Client = GetPrivateProperty<SteamClient>(app, "Client");

            request.SourceJobID = Client.GetNextJobID();
            request.Body.app_id = appid;
            request.Body.betapassword = betapassword;
            
            Client.Send( request );
            return request.SourceJobID;
        }
        
        public static void HandleBetaPasswordResponse(this SteamApps app, IPacketMsg packetMsg )
        {
            var passwordResponse = new ClientMsgProtobuf<CMsgClientCheckAppBetaPasswordResponse>( packetMsg );
            var callback = new BetaPasswordCallback(passwordResponse.TargetJobID, passwordResponse.Body);
            
            SteamClient Client = GetPrivateProperty<SteamClient>(app, "Client");
            Client.PostCallback( callback );
        }         
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
        public Dictionary<string, string> BetaPasswords { get; private set; }
        public Dictionary<Tuple<uint, string>, SteamApps.CDNAuthTokenCallback> CDNAuthTokens { get; private set; }
        public Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo> AppInfo { get; private set; }
        public Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo> PackageInfo { get; private set; }

        public SteamClient steamClient;
        public SteamUser steamUser;
        SteamApps steamApps;

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
            this.BetaPasswords = new Dictionary<string, string>();
            this.CDNAuthTokens = new Dictionary<Tuple<uint, string>, SteamApps.CDNAuthTokenCallback>();
            this.AppInfo = new Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo>();
            this.PackageInfo = new Dictionary<uint, SteamApps.PICSProductInfoCallback.PICSProductInfo>();

            this.steamClient = new SteamClient();

            this.steamUser = this.steamClient.GetHandler<SteamUser>();
            this.steamApps = this.steamClient.GetHandler<SteamApps>();

            // Kludge to add handler to SteamKit2 SteamApps
            // TODO: update SteamKit2 itself instead and then bump version requirement
            Dictionary<EMsg, Action<IPacketMsg>> dispatchMap = SteamAppsExtensions.GetPrivateField<Dictionary<EMsg, Action<IPacketMsg>>>(this.steamApps, "dispatchMap");    
            dispatchMap.Add((EMsg)5451, this.steamApps.HandleBetaPasswordResponse);

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

        public void CheckAppBetaPassword(uint appid, string betapassword)
        {
            bool completed = false;
            Action<SteamAppsExtensions.BetaPasswordCallback> cbMethod = (response) =>
            {
                completed = true;
                Console.WriteLine("Got beta branch key(s).");
                if (response.Result != EResult.OK)
                {
                    Abort();
                    return;
                }
                
                foreach (var entry in response.Password)
                {
                    this.BetaPasswords[entry.betaname] = entry.betapassword;
                }
            };
            
            WaitUntilCallback(() =>
            {
                callbacks.Subscribe(steamApps.GetBetaPassword(appid, betapassword), cbMethod);
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
