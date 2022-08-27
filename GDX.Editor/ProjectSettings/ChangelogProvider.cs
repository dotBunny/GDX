// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using GDX.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor
{
    public static class ChangelogProvider
    {
        enum ArtifactType
        {
            Default,
            Header1,
            Header2,
            Header3,
            Header4,
            Bold,
            Italic,
            BoldItalic,
            RightArrow,
            Link,
            DotAdded,
            DotChanged,
            DotRemoved,
            DotFixed,
            Item,
            ItemText,
            Badge,
            Spacer
        }
        enum ChangelogSection
        {
            Default = 0,
            Added = 1,
            Changed = 2,
            Fixed = 3,
            Removed = 4
        }

        static readonly int k_AddedLineHash = "### Added".GetStableHashCode();
        static readonly int k_ArrowHash = "->".GetStableHashCode();
        static readonly int k_ChangedLineHash = "### Changed".GetStableHashCode();
        static readonly int k_FixedLineHash = "### Fixed".GetStableHashCode();
        static readonly int k_RemovedLineHash = "### Removed".GetStableHashCode();
        static VisualElement s_TargetContainer;

        public static void StartTask(VisualElement container)
        {
            if (Config.EditorTaskDirectorSystem)
            {
                new ParseChangelogTask(container, OnTaskComplete).Enqueue();
                container.Clear();
                Label loadingLabel = new Label("Loading ...");
                loadingLabel.AddToClassList("gdx-changelog-loading");
                container.Add(loadingLabel);
            }
            else
            {
                new ParseChangelogTask(container, OnTaskComplete).Complete();
            }
        }

        static VisualElement CreateVisualElement(this Artifact artifact)
        {
            switch (artifact.Type)
            {
                case ArtifactType.Header1:
                    Label h1 = new Label(artifact.Content);
                    h1.AddToClassList("gdx-changelog-h1");
                    return h1;
                case ArtifactType.Header2:
                    Label h2 = new Label(artifact.Content);
                    h2.AddToClassList("gdx-changelog-h2");
                    return h2;
                case ArtifactType.Header3:
                    Label h3 = new Label(artifact.Content);
                    h3.AddToClassList("gdx-changelog-h3");
                    return h3;
                case ArtifactType.Header4:
                    Label h4 = new Label(artifact.Content);
                    h4.AddToClassList("gdx-changelog-h4");
                    return h4;
                case ArtifactType.Bold:
                    Label bold = new Label(artifact.Content);
                    bold.AddToClassList("gdx-changelog-bold");
                    return bold;
                case ArtifactType.Italic:
                    Label italic = new Label(artifact.Content);
                    italic.AddToClassList("gdx-changelog-italic");
                    return italic;
                case ArtifactType.BoldItalic:
                    Label boldItalic = new Label(artifact.Content);
                    boldItalic.AddToClassList("gdx-changelog-bold-italic");
                    return boldItalic;
                case ArtifactType.Link:
                    Button linkedButton = new Button { text = artifact.Content };
                    linkedButton.clicked += () => { Application.OpenURL(artifact.Extra); };
                    linkedButton.AddToClassList("gdx-link");
                    return linkedButton;
                case ArtifactType.RightArrow:
                    VisualElement arrow = new VisualElement();
                    arrow.AddToClassList("gdx-right-arrow");
                    return arrow;
                case ArtifactType.DotAdded:
                    VisualElement dotAddedSurround = new VisualElement();
                    dotAddedSurround.AddToClassList("dot-container");
                    VisualElement dotAdded = new VisualElement();
                    dotAddedSurround.Add(dotAdded);
                    dotAdded.AddToClassList("dot");
                    dotAdded.AddToClassList("dot-added");
                    return dotAddedSurround;
                case ArtifactType.DotRemoved:
                    VisualElement dotRemovedSurround = new VisualElement();
                    dotRemovedSurround.AddToClassList("dot-container");
                    VisualElement dotRemoved = new VisualElement();
                    dotRemovedSurround.Add(dotRemoved);
                    dotRemoved.AddToClassList("dot");
                    dotRemoved.AddToClassList("dot-removed");
                    return dotRemovedSurround;
                case ArtifactType.DotChanged:
                    VisualElement dotChangedSurround = new VisualElement();
                    dotChangedSurround.AddToClassList("dot-container");
                    VisualElement dotChanged = new VisualElement();
                    dotChangedSurround.Add(dotChanged);
                    dotChanged.AddToClassList("dot");
                    dotChanged.AddToClassList("dot-changed");
                    return dotChangedSurround;
                case ArtifactType.DotFixed:
                    VisualElement dotModifiedSurround = new VisualElement();
                    dotModifiedSurround.AddToClassList("dot-container");
                    VisualElement dotModified = new VisualElement();
                    dotModifiedSurround.Add(dotModified);
                    dotModified.AddToClassList("dot");
                    dotModified.AddToClassList("dot-fixed");
                    return dotModifiedSurround;
                case ArtifactType.Item:
                    VisualElement item = new VisualElement();
                    item.AddToClassList("gdx-changelog-item");

                    int itemChildCount = artifact.SubArtifacts.Count;
                    for (int i = 0; i < itemChildCount; i++)
                    {
                        item.Add(artifact.SubArtifacts[i].CreateVisualElement());
                    }

                    return item;
                case ArtifactType.ItemText:
                    VisualElement itemContainer = new VisualElement();
                    itemContainer.AddToClassList("text-container");
                    int itemTextCount = artifact.SubArtifacts.Count;
                    for (int i = 0; i < itemTextCount; i++)
                    {
                        itemContainer.Add(artifact.SubArtifacts[i].CreateVisualElement());
                    }

                    return itemContainer;
                case ArtifactType.Badge:
                    Label code = new Label(artifact.Content);
                    code.AddToClassList("gdx-badge");
                    return code;
                case ArtifactType.Spacer:
                    VisualElement spacer = new VisualElement { style = { width = 10, height = 10 } };
                    return spacer;
            }

            return new Label(artifact.Content);
        }

        static Artifact GenerateItem(string item, ChangelogSection section = ChangelogSection.Default,
            ArtifactType prefix = ArtifactType.Default)
        {
            Artifact artifact = new Artifact(null, ArtifactType.Item);
            if (prefix != ArtifactType.Default)
            {
                artifact.SubArtifacts.Add(new Artifact(null, prefix));
            }

            switch (section)
            {
                case ChangelogSection.Added:
                    artifact.SubArtifacts.Add(new Artifact(null, ArtifactType.DotAdded));
                    break;
                case ChangelogSection.Changed:
                    artifact.SubArtifacts.Add(new Artifact(null, ArtifactType.DotChanged));
                    break;
                case ChangelogSection.Fixed:
                    artifact.SubArtifacts.Add(new Artifact(null, ArtifactType.DotFixed));
                    break;
                case ChangelogSection.Removed:
                    artifact.SubArtifacts.Add(new Artifact(null, ArtifactType.DotRemoved));
                    break;
            }

            Artifact textContainer = new Artifact(null, ArtifactType.ItemText);
            int length = item.Length;

            StringBuilder newLine = new StringBuilder();
            bool insideCodeIndicator = false;
            int insideDontBreak = 0;
            for (int i = 0; i < length; i++)
            {
                if (item[i] == '[')
                {
                    insideDontBreak++;
                }

                if (item[i] == ']')
                {
                    insideDontBreak--;
                }

                if (item[i] == '`')
                {
                    if (!insideCodeIndicator)
                    {
                        if (newLine.Length > 0)
                        {
                            textContainer.SubArtifacts.Add(ParseRegularText(newLine.ToString().Trim()));
                            newLine.Clear();
                        }
                    }
                    else
                    {
                        textContainer.SubArtifacts.Add(new Artifact(newLine.ToString().Trim(), ArtifactType.Badge));
                        newLine.Clear();
                    }

                    insideCodeIndicator = !insideCodeIndicator;
                }
                else
                {
                    newLine.Append(item[i]);
                    if (item[i] == ' ' && newLine.Length > 0 && insideDontBreak == 0)
                    {
                        textContainer.SubArtifacts.Add(ParseRegularText(newLine.ToString().Trim()));
                        newLine.Clear();
                    }
                }
            }

            if (newLine.Length > 0)
            {
                textContainer.SubArtifacts.Add(ParseRegularText(newLine.ToString().Trim()));
                newLine.Clear();
            }

            artifact.SubArtifacts.Add(textContainer);
            return artifact;
        }

        static void OnTaskComplete(TaskBase task)
        {
            ParseChangelogTask changelog = (ParseChangelogTask)task;
            changelog.Target.Clear();

            // Have to do the UIToolkit portion on the MT
            int count = changelog.Artifacts.Count;
            for (int i = 0; i < count; i++)
            {
                changelog.Target.Add(changelog.Artifacts[i].CreateVisualElement());
            }
        }

        static Artifact ParseRegularText(string text)
        {
            int hash = text.GetStableHashCode();

            // Right quick arrow
            if (hash == k_ArrowHash)
            {
                return new Artifact(null, ArtifactType.RightArrow);
            }

            // Link
            if (text.StartsWith("[") && text.EndsWith(")") && text.Contains("]("))
            {
                return new Artifact(text.Substring(1, text.IndexOf("]", StringComparison.Ordinal) - 1),
                    text.Substring(text.IndexOf("](", StringComparison.Ordinal) + 2).TrimEnd(')'), ArtifactType.Link);
            }

            // text
            return new Artifact(text);
        }

        struct Artifact
        {
            public readonly ArtifactType Type;
            public readonly string Content;
            public readonly string Extra;
            public readonly List<Artifact> SubArtifacts;

            public Artifact(string content)
            {
                Type = ArtifactType.Default;
                Content = content;
                Extra = null;
                SubArtifacts = new List<Artifact>();
            }

            public Artifact(string content, ArtifactType type)
            {
                Type = type;
                Content = content;
                Extra = null;

                if (type == ArtifactType.Item || type == ArtifactType.ItemText)
                {
                    SubArtifacts = new List<Artifact>();
                }
                else
                {
                    SubArtifacts = null;
                }
            }

            public Artifact(string content, string extra, ArtifactType type)
            {
                Type = type;
                Content = content;
                Extra = extra;
                SubArtifacts = null;
            }
        }

        class ParseChangelogTask : TaskBase
        {
            public readonly List<Artifact> Artifacts = new List<Artifact>();
            public readonly VisualElement Target;

            public ParseChangelogTask(VisualElement target, Action<TaskBase> onCompletedMainThread)
            {
                Target = target;
                m_Name = "GDX Changelog";
                m_IsLogging = false;
                completedMainThread = onCompletedMainThread;
            }

            /// <inheritdoc />
            public override void DoWork()
            {
                ChangelogSection section = ChangelogSection.Default;

                string[] changelog = UpdateProvider.GetLocalChangelog(3);
                int lineCount = changelog.Length;

                Artifacts.Add(new Artifact("Changelog", ArtifactType.Header1));
                for (int i = 0; i < lineCount; i++)
                {
                    string s = changelog[i];

                    // Determine sections
                    int lineHash = s.GetStableHashCode();
                    if (lineHash == k_AddedLineHash)
                    {
                        section = ChangelogSection.Added;
                        Artifacts.Add(new Artifact("Added", ArtifactType.Header2));
                        i++; // skip next line
                        continue;
                    }

                    if (lineHash == k_ChangedLineHash)
                    {
                        section = ChangelogSection.Changed;
                        Artifacts.Add(new Artifact("Changed", ArtifactType.Header2));
                        i++; // skip next line
                        continue;
                    }

                    if (lineHash == k_FixedLineHash)
                    {
                        section = ChangelogSection.Fixed;
                        Artifacts.Add(new Artifact("Fixed", ArtifactType.Header2));
                        i++; // skip next line
                        continue;
                    }

                    if (lineHash == k_RemovedLineHash)
                    {
                        section = ChangelogSection.Removed;
                        Artifacts.Add(new Artifact("Removed", ArtifactType.Header2));
                        i++; // skip next line
                        continue;
                    }

                    // Special Cases
                    if (s.StartsWith("## "))
                    {
                        Artifacts.Add(new Artifact(s.Replace("## ", string.Empty),
                            ArtifactType.Header2));
                        i++; // skip next line
                        continue;
                    }

                    if (s.StartsWith("***"))
                    {
                        Artifacts.Add(new Artifact(
                            s.Replace("***\"", string.Empty)
                                .Replace("\"***", string.Empty).Trim(), ArtifactType.BoldItalic));
                        continue;
                    }

                    if (s.StartsWith("> "))
                    {
                        Artifacts.Add(new Artifact(s.Replace("> ", string.Empty),
                            ArtifactType.Italic));
                        continue;
                    }

                    if (s.StartsWith("- "))
                    {
                        Artifacts.Add(GenerateItem(s.Replace("- ", string.Empty), section));
                        continue;
                    }

                    if (s.StartsWith("  - "))
                    {
                        Artifacts.Add(GenerateItem(s.Replace("  - ", string.Empty), section,
                            ArtifactType.Spacer));
                        continue;
                    }

                    Artifacts.Add(new Artifact(s));
                }
            }
        }
    }
}