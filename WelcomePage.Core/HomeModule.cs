﻿using System.Diagnostics;
using System.Reflection;
using MarkdownDeep;
using Nancy;
using Nancy.Responses.Negotiation;

namespace WelcomePage.Core
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IDocumentFolder documentFolder)
        {
            Get["/"] = x => GetDocument(documentFolder, "README");
            Get["/{path*}"] = x => GetDocument(documentFolder, (string)x.Path);
            Get["/_About"] = x => GetAbout();
        }

        private Negotiator GetAbout()
        {
            var processId = Process.GetCurrentProcess().Id;
            var location = Assembly.GetExecutingAssembly().Location;
            var informationalVersion =
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version =
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
            var model =
                new
                    {
                        ProcessId = processId,
                        Location = location,
                        Version =
                            informationalVersion != null
                                ? informationalVersion.InformationalVersion
                                : version.Version
                    };
            return View["About", model];
        }

        private Negotiator GetDocument(IDocumentFolder documentFolder, string path)
        {
            var markdown = documentFolder.ReadAllText(path);
            var converter = new Markdown();
            var html = converter.Transform(markdown);
            return View["Index", new { Title = path, Content = html }];
        }
    }
}