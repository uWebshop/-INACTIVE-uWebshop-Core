using System;
using System.Reflection;
using System.Web;
using umbraco.BusinessLogic;

namespace uWebshop.ActionHandlers
{
      /// <summary>
   /// Since Umbraco release 4.8 we have some breaking changes with 3th party dlls
   /// This class will make sure the proper version gets loaded in case of an loading error
   /// More info on http://stackoverflow.com/questions/2664028/how-can-i-get-powershell-added-types-to-use-added-types/2669782#2669782
   /// </summary>
   public class UpdateThirdPartyDllBindings : ApplicationBase
   {
       public UpdateThirdPartyDllBindings()
       {
           AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
       }

       /// <summary>
       /// Assembly could not be resolved let's see if we can load it dynamically
       /// </summary>
       static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
       {
           var legacyFile = HttpContext.Current.Server.MapPath("~/bin/legacy/Lucene.Net.dll");


           if (args.Name == "Lucene.Net, Version=2.9.2.2, Culture=neutral, PublicKeyToken=f5940d1699e37ff1")
           {
               try
               {
                   return Assembly.LoadFile(legacyFile);
               }
               catch(Exception ex)
               {
                   Log.Add(LogTypes.Error, -1,
                           string.Format("uWebshop error loading Lucene.net version {0}:{1} ", legacyFile, ex));
               }
           }

           return null;
       }
   }
}