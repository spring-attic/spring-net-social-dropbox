using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Spring.IO;
using Spring.Social.OAuth1;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;

namespace Spring.OAuth1ConsoleQuickStart
{
    class Program
    {
        // Register your own Dropbox app at https://www.dropbox.com/developers/apps with "Full Dropbox" access level.
        // Set your consumer key & secret here
        private const string DropboxAppKey = TODO;
        private const string DropboxAppSecret = TODO;

        static void Main(string[] args)
        {
            try
            {
                DropboxServiceProvider dropboxServiceProvider = new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret, AccessLevel.Full);

#if NET_4_0
                /* OAuth 1.0 'dance' */

                // Authorization without callback url
                Console.Write("Getting request token...");
                OAuthToken oauthToken = dropboxServiceProvider.OAuthOperations.FetchRequestTokenAsync(null , null).Result;
                Console.WriteLine("Done");

                OAuth1Parameters parameters = new OAuth1Parameters();
                //parameters.Add("locale", CultureInfo.CurrentUICulture.IetfLanguageTag); // for a localized version of the authorization website
                string authenticateUrl = dropboxServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, parameters);
                Console.WriteLine("Redirect user for authorization");
                Process.Start(authenticateUrl);
                Console.Write("Press any key when authorization attempt has succeeded");
                Console.ReadLine();

                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, null);
                OAuthToken oauthAccessToken = dropboxServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
                Console.WriteLine("Done");

                /* API */

                IDropbox dropbox = dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                //dropbox.Locale = CultureInfo.CurrentUICulture.IetfLanguageTag;

                DropboxProfile profile = dropbox.GetUserProfileAsync().Result;
                Console.WriteLine("Hi " + profile.DisplayName + "!");

                // Use step by step debugging, or not             
/*
                DeltaPage deltaPage = dropbox.DeltaAsync(null).Result;
                Entry createFolderEntry = dropbox.CreateFolderAsync("Spring Social").Result;
                Entry uploadFileEntry = dropbox.UploadFileAsync(
                    new AssemblyResource("assembly://Spring.ConsoleQuickStart/Spring.ConsoleQuickStart/File.txt"),
                    "/Spring Social/File.txt", true, null, CancellationToken.None).Result;
                FileRef fileRef = dropbox.CreateFileRefAsync("Spring Social/File.txt").Result;
                Entry copyRefEntry = dropbox.CopyFileRefAsync(fileRef.Value, "Spring Social/File_copy_ref.txt").Result;
                Entry copyEntry = dropbox.CopyAsync("Spring Social/File.txt", "Spring Social/File_copy.txt").Result;
                Entry deleteEntry = dropbox.DeleteAsync("Spring Social/File.txt").Result;
                Entry moveEntry = dropbox.MoveAsync("Spring Social/File_copy.txt", "Spring Social/File.txt").Result;
                dropbox.DownloadFileAsync("Spring Social/File.txt")
                    .ContinueWith(task =>
                    {
                        Console.WriteLine("File '{0}' downloaded ({1})", task.Result.Metadata.Path, task.Result.Metadata.Size);
                        // Save file to "C:\Spring Social.txt"
                        using (FileStream fileStream = new FileStream(@"C:\Spring Social.txt", FileMode.Create))
                        {
                            fileStream.Write(task.Result.Content, 0, task.Result.Content.Length);
                        }
                    });
                Entry folderMetadata = dropbox.GetMetadataAsync("Spring Social").Result;
                IList<Entry> revisionsEntries = dropbox.GetRevisionsAsync("Spring Social/File.txt").Result;
                Entry restoreEntry = dropbox.RestoreAsync("Spring Social/File.txt", revisionsEntries[2].Revision).Result;
                IList<Entry> searchResults = dropbox.SearchAsync("Spring Social/", ".txt").Result;
                DropboxLink shareableLink = dropbox.GetShareableLinkAsync("Spring Social/File.txt").Result;
                DropboxLink mediaLink = dropbox.GetMediaLinkAsync("Spring Social/File.txt").Result;
                Entry uploadImageEntry = dropbox.UploadFileAsync(
                    new AssemblyResource("assembly://Spring.ConsoleQuickStart/Spring.ConsoleQuickStart/Image.png"),
                    "/Spring Social/Image.png", true, null, CancellationToken.None).Result;
                dropbox.DownloadThumbnailAsync("Spring Social/Image.png", ThumbnailFormat.Png, ThumbnailSize.Medium)
                    .ContinueWith(task =>
                    {
                        Console.WriteLine("Thumbnail '{0}' downloaded ({1})", task.Result.Metadata.Path, task.Result.Metadata.Size);
                        // Save file to "C:\Thumbnail_Medium.png"
                        using (FileStream fileStream = new FileStream(@"C:\Thumbnail_Medium.png", FileMode.Create))
                        {
                            fileStream.Write(task.Result.Content, 0, task.Result.Content.Length);
                        }
                    });
*/
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                    {
                        if (ex is DropboxApiException)
                        {
                            Console.WriteLine(ex.Message);
                            return true;
                        }
                        return false;
                    });
            }
#else
                /* OAuth 1.0 'dance' */

                // Authorization without callback url
                Console.Write("Getting request token...");
                OAuthToken oauthToken = dropboxServiceProvider.OAuthOperations.FetchRequestToken(null, null);
                Console.WriteLine("Done");

                OAuth1Parameters parameters = new OAuth1Parameters();
                //parameters.Add("locale", CultureInfo.CurrentUICulture.IetfLanguageTag); // for a localized version of the authorization website
                string authenticateUrl = dropboxServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, parameters);
                Console.WriteLine("Redirect user for authorization");
                Process.Start(authenticateUrl);
                Console.Write("Press any key when authorization attempt has succeeded");
                Console.ReadLine();

                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, null);
                OAuthToken oauthAccessToken = dropboxServiceProvider.OAuthOperations.ExchangeForAccessToken(requestToken, null);
                Console.WriteLine("Done");

                /* API */

                IDropbox dropbox = dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                //dropbox.Locale = CultureInfo.CurrentUICulture.IetfLanguageTag;

                DropboxProfile profile = dropbox.GetUserProfile();
                Console.WriteLine("Hi " + profile.DisplayName + "!");

                // Use step by step debugging, or not
/*
                DeltaPage deltaPage = dropbox.Delta(null);
                Entry createFolderEntry = dropbox.CreateFolder("Spring Social");
                Entry uploadFileEntry = dropbox.UploadFile(
                    new AssemblyResource("assembly://Spring.ConsoleQuickStart/Spring.ConsoleQuickStart/File.txt"),
                    "/Spring Social/File.txt", true, null);
                FileRef fileRef = dropbox.CreateFileRef("Spring Social/File.txt");
                Entry copyRefEntry = dropbox.CopyFileRef(fileRef.Value, "Spring Social/File_copy_ref.txt");
                Entry copyEntry = dropbox.Copy("Spring Social/File.txt", "Spring Social/File_copy.txt");
                Entry deleteEntry = dropbox.Delete("Spring Social/File.txt");
                Entry moveEntry = dropbox.Move("Spring Social/File_copy.txt", "Spring Social/File.txt");
                var fileCanceler = dropbox.DownloadFileAsync("Spring Social/File.txt",
                    r =>
                    {
                        Console.WriteLine("File '{0}' downloaded ({1})", r.Response.Metadata.Path, r.Response.Metadata.Size);
                        // Save file to "C:\Spring Social.txt"
                        using (FileStream fileStream = new FileStream(@"C:\Spring Social.txt", FileMode.Create))
                        {
                            fileStream.Write(r.Response.Content, 0, r.Response.Content.Length);
                        }
                    });
                Entry folderMetadata = dropbox.GetMetadata("Spring Social");
                IList<Entry> revisionsEntries = dropbox.GetRevisions("Spring Social/File.txt");
                Entry restoreEntry = dropbox.Restore("Spring Social/File.txt", revisionsEntries[2].Revision);
                IList<Entry> searchResults = dropbox.Search("Spring Social/", ".txt");
                DropboxLink shareableLink = dropbox.GetShareableLink("Spring Social/File.txt");
                DropboxLink mediaLink = dropbox.GetMediaLink("Spring Social/File.txt");
                Entry uploadImageEntry = dropbox.UploadFile(
                    new AssemblyResource("assembly://Spring.ConsoleQuickStart/Spring.ConsoleQuickStart/Image.png"),
                    "/Spring Social/Image.png", true, null);
                var thumbnailCanceler = dropbox.DownloadThumbnailAsync("Spring Social/Image.png", ThumbnailFormat.Png, ThumbnailSize.Medium, 
                    r =>
                    {
                        Console.WriteLine("Thumbnail '{0}' downloaded ({1})", r.Response.Metadata.Path, r.Response.Metadata.Size);
                        // Save file to "C:\Thumbnail_Medium.png"
                        using (FileStream fileStream = new FileStream(@"C:\Thumbnail_Medium.png", FileMode.Create))
                        {
                            fileStream.Write(r.Response.Content, 0, r.Response.Content.Length);
                        }
                    });
*/
            }
            catch (DropboxApiException ex)
            {
                Console.WriteLine(ex.Message);
            }
#endif
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("--- hit <return> to quit ---");
                Console.ReadLine();
            }
        }
    }
}