using Google.Cloud.Firestore;
using Godot;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhackAMath
{
	internal class FirestoreHelper : IDataService
	{
		private static FirestoreHelper _instance;
        public static FirestoreHelper Instance
        {
            get
            {
                _instance ??= new FirestoreHelper();
                return _instance;
            }
        }

        private FirestoreHelper() { }

		static string FileConfig = @"
		{
		  ""type"": ""service_account"",
		  ""project_id"": ""whack-a-math-fddc0"",
		  ""private_key_id"": ""887787d0482b9d3955460ff146434045d2485d63"",
		  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC08Iyz2YiukSpT\n8pbRp282W/185Q9EGb7lSy6ClB+HST04YS6Gs1J5R35OBxcFLuXqPMurL6pRUgzs\nFIIXhOLKCBan0F2AsClhDkEuVKm8bPy5AQVGegPYQiSjbWJUszN21eglXd5kjiEX\nns9Ux82CzX4SPaq7Hhhw/erXKsWOBDbPS+9GigrFdeG25enD8WcugTH2t8oe4CEm\nshOscpkSsltKIZa5TIR6cKtAXPW7pKoM+qElqXMACr1LVREo75ToWR0sJgU3t/ry\nWedxWJoj1uik1vZhybrgCRmMt4gFgeA+1hgVxwJJrLJEeDODQcZlRdUfafwO/5Pm\n/dKPx43JAgMBAAECggEATKweQ1vr0mVLNXV3uXGk67kBano6BNaQEOPR2p1f2tkL\nKyfrKkM0sJW+DNxuQdEEtkR63Zh+KKWHOkbadZLm80uIJiZiaNS9RBZhQnu3zVO1\np8Op85pipLIqimIgp9mj9jhgfe0P/zZHCZZPLxLXoBTp1lrxTQdMvhyq9fB29F1X\nsT9c2JXn2OsEG0UY8rs2pbz5sDRT5cQ2oUyZpVD9nOq0Es6aCiEUK9MZoi9x7t0N\nSZwb2tVkkBfO9IWDw5YwNAZzz7zrOgsqCMMys6UNCiDMIHbn4ErotPCgtZFMr4In\nBAA4Zk5CGhHn2AXy7JhFVTbarmuCWrk+bNI0s7pvLQKBgQDqws3GP/m+jqUCs8xs\ntRwWXFMu1UoyVtoooEL3s/hklV9EbiY3EpzqAIW0zJPSGFMDjCDyQxgw/NMoI4mq\nvZItPW55ovjvMLVFgrJsryELno4NC9gCrsFGcKAjOeLGwDr6y1z6CkYjZDrpUT8+\nKHkxMKyziBScMe/2N96puo7HCwKBgQDFTzbIUUVI6mXbM8w1vOjA5Z2DIi8Qh8MK\n64dHry+45GG1o5G5hbYQCZdI0JEQQiPq0qicMOIk88EzkWmz0vgd5FvQEUSCi4KV\nZOmuQzEbWvvKd7x1I+KDNEf0c1DsJX5wzUsmx2uqOrjxAQiJEnI1YJX/eykoF0mD\nsCFBpiby+wKBgQDhQQe2uL4GpNIZ1LFKgUCxB+dc4ZTu1j0/i/6VtF6bDGQ7wcDz\nSO287cFdaIpRpbtJhRQ7wDsfDggB1I+Mf5cZx38QQVrADylvx+cxt5xqjLlaNUoP\n0ORslTZidnFtKReueqD83gzMeqi4JJgdKTKYC086eFDmSlgoFRWWIAZNXwKBgCnj\nJDUok7XkFRWRDIRIA32YAXVqV2WB/NUfpUuBlHcC/P9Lp08kZL9I2fYaWQGDmQ5s\nBGGMOyYvHU/uSetkGUp3RUqJr+qq5WED2FwnZZByI0wbWUXhBWBnQ/NQW9iWKR6X\nZqHn6iJsbwCzAHXhFZ/hTla7dcROFUxAov7F4DTrAoGAdv66i0dd7x5tZdf0qec2\nPEMRlpLSQQ98wiCZImkYDmsajHVWaUDdmGnT5saKqOlX11tv1qJLcoxGOODO/V38\nkYtDQGFkVvIE/4y1vc5/aMyMZWxH7BFlEunTRmdg3nEjoE9NLnGK/3sYhA+Thazm\nQeXyIZKYMrllhdDSaV7d1MM=\n-----END PRIVATE KEY-----\n"",
		  ""client_email"": ""firebase-adminsdk-lphgl@whack-a-math-fddc0.iam.gserviceaccount.com"",
		  ""client_id"": ""107962305313996360570"",
		  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
		  ""token_uri"": ""https://oauth2.googleapis.com/token"",
		  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
		  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-lphgl%40whack-a-math-fddc0.iam.gserviceaccount.com"",
		  ""universe_domain"": ""googleapis.com""
		}";
		static string Filepath = "";
        public static FirestoreDb Database { get; private set; }
        public static FirebaseAuthClient Auth { get; private set; }
        public static UserCredential UserCredential { get; private set; }
        public static CollectionReference CollectionRef { get; private set; }
        public static string CollectionName = "SaveData";

		public static void SetEnvironmentVariable()
		{
			Filepath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(Filepath, FileConfig);
            File.SetAttributes(Filepath, FileAttributes.Hidden);
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Filepath);
            Database = FirestoreDb.Create("whack-a-math-fddc0");

			var config = new FirebaseAuthConfig
			{
				ApiKey = "AIzaSyDCKNsHZyc0-PULE22oqpiY6lTBGPZmNcI",
				AuthDomain = "whack-a-math-fddc0.firebaseapp.com",
				Providers = new FirebaseAuthProvider[]
				{
					// Add and configure individual providers
					new EmailProvider()
				},
				UserRepository = new FileUserRepository("FirebaseSample") // persist data into %AppData%\FirebaseSample
			};

			Auth = new FirebaseAuthClient(config);
            CollectionRef = Database.Collection(CollectionName);
            File.Delete(Filepath);
		}

		public static async Task<UserCredential> AuthenticateUser(string email, string password)
        {
            return UserCredential = await Auth.SignInWithEmailAndPasswordAsync(email, password);
        }

        public static async Task<UserCredential> CreateUser(string email, string password)
        {
            return UserCredential = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        }

        public static async Task SendPasswordResetEmailAsync(string email)
        {
            await Auth.ResetEmailPasswordAsync(email);
        }

        public static async Task DeleteUserAsync(string email, string password)
        {
            var user = await Auth.SignInWithEmailAndPasswordAsync(email, password);
            if (user.User.Info.Uid != UserCredential.User.Info.Uid)
            {
                throw new FirebaseAuthException("INVALID_LOGIN_CREDENTIALS", AuthErrorReason.Unknown);
            }
            else
            {
                await CollectionRef.Document(UserCredential.User.Info.Uid).DeleteAsync();
                await UserCredential.User.DeleteAsync();
                SaveFile.InitialSaveFile();
            }
        }

        public static void SignOut()
        {
            Auth.SignOut();
            SaveFile.InitialSaveFile();
        }

        public static async Task ChangePassword(string email, string oldPassword, string newPassword)
        {
            var user = await Auth.SignInWithEmailAndPasswordAsync(email, oldPassword);
            if (user.User.Info.Uid != UserCredential.User.Info.Uid)
            {
                throw new FirebaseAuthException("INVALID_LOGIN_CREDENTIALS", AuthErrorReason.Unknown);
            }
            else
            {
                await UserCredential.User.ChangePasswordAsync(newPassword);
            }
        }

		public static async Task CreateDocument(Dictionary<string, object> data)
		{
			var documentRef = Database.Collection(CollectionName).Document(Auth.User.Info.Uid);
			await documentRef.SetAsync(SaveFile.ConvertToDictionary());
		}

		public static async Task UpdateDocument(Dictionary<string, object> data)
		{
			if (Auth?.User?.Info?.Uid == null)
			{
				GD.PrintErr("Auth.User.Info.Uid is null. Cannot update document.");
				return;
			}

			var documentRef = Database.Collection(CollectionName).Document(Auth.User.Info.Uid);

			if (documentRef == null)
			{
				GD.PrintErr("documentRef is null. Cannot update document.");
				return;
			}

			try
			{
				await documentRef.UpdateAsync(data);
				GD.Print("Document updated successfully.");
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Failed to update document: {ex.Message}");
				throw;
			}
		}

		public static async Task<Dictionary<string, object>> GetDocument()
		{
			var documentRef = Database.Collection(CollectionName).Document(Auth.User.Info.Uid);
			DocumentSnapshot snapshot = await documentRef.GetSnapshotAsync();
			if (snapshot.Exists)
			{
				return snapshot.ToDictionary();
			}
			else
			{
				return null;
			}
		}

        public async Task SaveGameDataAsync(Dictionary<string, object> data)
        {
            var documentRef = Database.Collection(CollectionName).Document(Auth.User.Info.Uid);
            await documentRef.SetAsync(data);
        }

        public async Task<Dictionary<string, object>> LoadGameDataAsync()
        {
            var documentRef = Database.Collection(CollectionName).Document(Auth.User.Info.Uid);
            DocumentSnapshot snapshot = await documentRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ToDictionary();
            }
            else
            {
                return null;
            }
        }

        public void Logout()
		{
			SignOut();
		}
    }
}
