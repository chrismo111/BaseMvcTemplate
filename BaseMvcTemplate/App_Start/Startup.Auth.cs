﻿using BaseMvcTemplate.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Owin.Security.Providers.LinkedIn;
using System;
using System.Web.Configuration;

namespace BaseMvcTemplate {

    public partial class Startup {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth( IAppBuilder app ) {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext( ApplicationDbContext.Create );
            app.CreatePerOwinContext<ApplicationUserManager>( ApplicationUserManager.Create );
            app.CreatePerOwinContext<ApplicationSignInManager>( ApplicationSignInManager.Create );

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication( new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString( "/Account/Login" ),
                Provider = new CookieAuthenticationProvider {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval : TimeSpan.FromMinutes( 30 ),
                        regenerateIdentity : ( manager, user ) => user.GenerateUserIdentityAsync( manager ) )
                }
            } );
            app.UseExternalSignInCookie( DefaultAuthenticationTypes.ExternalCookie );

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie( DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes( 5 ) );

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie( DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie );
            app.UseFacebookAuthentication( appId : WebConfigurationManager.AppSettings["facebookAuthAppId"], appSecret : WebConfigurationManager.AppSettings["facebookAuthAppSecret"] );
            app.UseGoogleAuthentication( WebConfigurationManager.AppSettings["googleAuthClientID"], WebConfigurationManager.AppSettings["googleAuthClientSecret"] );
            app.UseLinkedInAuthentication( WebConfigurationManager.AppSettings["linkedInAuthClientId"], WebConfigurationManager.AppSettings["linkedInAuthSecret"] );

            Seed();
        }

        private void Seed() {

            using( var ctx = new ApplicationDbContext() ) {
                using( var roleMgr = new RoleManager<IdentityRole>( new RoleStore<IdentityRole>( ctx ) ) ) {
                    if( !roleMgr.RoleExists( "Administrator" ) ) {
                        roleMgr.Create( new IdentityRole( "Administrator" ) );
                    }

                    if( !roleMgr.RoleExists( "Submitter" ) ) {
                        roleMgr.Create( new IdentityRole( "Submitter" ) );
                    }
                }

                using( var userMgr = new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( ctx ) ) ) {

                    Action<string, string, string, string> createUser = ( userName, email, pass, role ) => {
                        var user = new ApplicationUser { UserName = userName, Email = email };
                        if( userMgr.FindByName( userName ) == null ) {
                            var result = userMgr.Create( user, pass );
                            if( result.Succeeded ) {
                                userMgr.AddToRole( user.Id, role );
                            }
                        }
                    };

                    createUser( "Admin", WebConfigurationManager.AppSettings["adminEmail"], WebConfigurationManager.AppSettings["adminPass"], "Administrator" );
                    createUser( "Submitter", WebConfigurationManager.AppSettings["submitterEmail"], WebConfigurationManager.AppSettings["submitterPass"], "Submitter" );
                }
            }
        }

    }
}