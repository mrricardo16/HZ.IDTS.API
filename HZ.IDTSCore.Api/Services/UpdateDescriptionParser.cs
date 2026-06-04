#nullable disable

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HZ.IDTSCore.Api.Services
{
    public class ParsedUpdateDescription
    {
        public string ReleaseVersion { get; set; }

        public List<ParsedUpdateItem> Items { get; set; } = new List<ParsedUpdateItem>();

        public Dictionary<string, string> ContentMap
        {
            get
            {
                return Items.ToDictionary(it => it.PackageVersion, it => it.UpdateContent);
            }
        }
    }

    public class ParsedUpdateItem
    {
        public string Sequence { get; set; }

        public string PackageVersion { get; set; }

        public string PackageTypeKey { get; set; }

        public string UpdateContent { get; set; }
    }

    public static class UpdateDescriptionParser
    {
        private static readonly Regex SequenceRegex = new Regex(@"^\s*序号\s*[:：]\s*(.*)\s*$", RegexOptions.Compiled);
        private static readonly Regex ReleaseRegex = new Regex(@"^\s*更新发布\s*[:：]\s*(.*)\s*$", RegexOptions.Compiled);
        private static readonly Regex VersionRegex = new Regex(@"^\s*版本号\s*[:：]\s*(.+)\s*$", RegexOptions.Compiled);

        public static bool IsUnifiedDescriptionPath(string entryName)
        {
            string normalizedName = NormalizeEntryName(entryName);
            string[] parts = normalizedName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 1 && parts.Length != 2)
            {
                return false;
            }
            return string.Equals(parts[parts.Length - 1], "更新说明.txt", StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryGetPackageDirectory(string entryName, out string packageTypeKey, out string packageVersion)
        {
            packageTypeKey = string.Empty;
            packageVersion = string.Empty;

            string normalizedName = NormalizeEntryName(entryName);
            string[] parts = normalizedName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return false;
            }

            if (IsPackageTypeKey(parts[0]) && parts.Length >= 2)
            {
                packageTypeKey = parts[0].ToLower();
                packageVersion = parts[1];
                return !string.IsNullOrEmpty(packageVersion);
            }

            if (parts.Length >= 3 && IsPackageTypeKey(parts[1]))
            {
                packageTypeKey = parts[1].ToLower();
                packageVersion = parts[2];
                return !string.IsNullOrEmpty(packageVersion);
            }

            return false;
        }

        public static bool TryParseFromZip(string zipPath, out ParsedUpdateDescription description, out string errorMessage)
        {
            description = null;
            errorMessage = string.Empty;

            try
            {
                byte[] descriptionBytes = ReadDescriptionBytes(zipPath);
                if (descriptionBytes == null)
                {
                    errorMessage = "更新包中未找到支持的更新说明文件";
                    return false;
                }

                string content;
                if (!TryDecodeDescription(descriptionBytes, out content))
                {
                    errorMessage = "更新说明.txt中文读取失败，请确认文件编码为UTF-8或GBK。";
                    return false;
                }

                return TryParse(content, out description, out errorMessage);
            }
            catch (Exception ex)
            {
                description = null;
                errorMessage = "更新说明.txt解析异常：" + ex.Message;
                return false;
            }
        }

        private static byte[] ReadDescriptionBytes(string zipPath)
        {
            using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(zipPath)))
            {
                ZipEntry theEntry;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var gbk = Encoding.GetEncoding("GBK");
                ZipStrings.CodePage = gbk.CodePage;

                while ((theEntry = zipInputStream.GetNextEntry()) != null)
                {
                    if (theEntry.IsDirectory || !IsUnifiedDescriptionPath(theEntry.Name))
                    {
                        continue;
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[2048];
                        int size;
                        while ((size = zipInputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, size);
                        }
                        return memoryStream.ToArray();
                    }
                }
            }

            return null;
        }

        private static bool TryDecodeDescription(byte[] bytes, out string content)
        {
            content = string.Empty;

            try
            {
                UTF8Encoding utf8 = new UTF8Encoding(false, true);
                content = utf8.GetString(bytes);
                return true;
            }
            catch (DecoderFallbackException)
            {
            }

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                content = Encoding.GetEncoding("GBK").GetString(bytes);
                return true;
            }
            catch
            {
                content = string.Empty;
                return false;
            }
        }

        private static bool TryParse(string content, out ParsedUpdateDescription description, out string errorMessage)
        {
            description = new ParsedUpdateDescription();
            errorMessage = string.Empty;

            string[] lines = content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            ItemBuilder currentItem = null;
            int itemIndex = 0;
            bool collectingContent = false;

            foreach (string rawLine in lines)
            {
                string line = rawLine.TrimEnd();
                if (IsSeparatorLine(line))
                {
                    if (currentItem != null)
                    {
                        if (!TryAppendItem(description, currentItem, itemIndex, out errorMessage))
                        {
                            return false;
                        }
                        currentItem = null;
                        collectingContent = false;
                    }
                    break;
                }

                Match releaseMatch = ReleaseRegex.Match(line);
                if (releaseMatch.Success)
                {
                    description.ReleaseVersion = releaseMatch.Groups[1].Value.Trim();
                    continue;
                }

                Match sequenceMatch = SequenceRegex.Match(line);
                if (sequenceMatch.Success)
                {
                    if (currentItem != null && !TryAppendItem(description, currentItem, itemIndex, out errorMessage))
                    {
                        return false;
                    }

                    itemIndex++;
                    currentItem = new ItemBuilder
                    {
                        Sequence = string.IsNullOrWhiteSpace(sequenceMatch.Groups[1].Value)
                            ? itemIndex.ToString()
                            : sequenceMatch.Groups[1].Value.Trim()
                    };
                    collectingContent = false;
                    continue;
                }

                if (currentItem == null)
                {
                    continue;
                }

                Match versionMatch = VersionRegex.Match(line);
                if (versionMatch.Success)
                {
                    currentItem.PackageVersion = versionMatch.Groups[1].Value.Trim();
                    collectingContent = true;
                    continue;
                }

                if (collectingContent)
                {
                    currentItem.ContentLines.Add(line);
                }
            }

            if (currentItem != null && !TryAppendItem(description, currentItem, itemIndex, out errorMessage))
            {
                return false;
            }

            if (description.Items.Count == 0)
            {
                errorMessage = "更新说明.txt格式异常，未读取到任何升级项。";
                return false;
            }

            return true;
        }

        private static bool TryAppendItem(ParsedUpdateDescription description, ItemBuilder itemBuilder, int itemIndex, out string errorMessage)
        {
            errorMessage = string.Empty;
            string sequence = string.IsNullOrEmpty(itemBuilder.Sequence) ? itemIndex.ToString() : itemBuilder.Sequence;
            if (string.IsNullOrWhiteSpace(itemBuilder.PackageVersion))
            {
                errorMessage = "更新说明.txt第" + sequence + "项缺少版本号。";
                return false;
            }

            string packageTypeKey;
            if (!TryGetPackageTypeKey(itemBuilder.PackageVersion, out packageTypeKey))
            {
                errorMessage = "更新说明.txt中版本号无法识别包类型：" + itemBuilder.PackageVersion;
                return false;
            }

            string updateContent = string.Join(Environment.NewLine, itemBuilder.ContentLines).Trim();
            if (string.IsNullOrWhiteSpace(updateContent))
            {
                errorMessage = "更新说明.txt中版本号" + itemBuilder.PackageVersion + "缺少更新说明正文。";
                return false;
            }

            description.Items.Add(new ParsedUpdateItem
            {
                Sequence = sequence,
                PackageVersion = itemBuilder.PackageVersion,
                PackageTypeKey = packageTypeKey,
                UpdateContent = updateContent
            });
            return true;
        }

        private static bool TryGetPackageTypeKey(string packageVersion, out string packageTypeKey)
        {
            packageTypeKey = string.Empty;
            if (packageVersion.StartsWith("idts_api_", StringComparison.OrdinalIgnoreCase))
            {
                packageTypeKey = "idts_api";
                return true;
            }
            if (packageVersion.StartsWith("idts_ui_", StringComparison.OrdinalIgnoreCase))
            {
                packageTypeKey = "idts_ui";
                return true;
            }
            if (packageVersion.StartsWith("idts_pgsql_", StringComparison.OrdinalIgnoreCase))
            {
                packageTypeKey = "idts_pgsql";
                return true;
            }
            return false;
        }

        private static bool IsPackageTypeKey(string value)
        {
            return string.Equals(value, "idts_api", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "idts_ui", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "idts_pgsql", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSeparatorLine(string line)
        {
            string trimLine = line.Trim();
            return trimLine.Length >= 8 && trimLine.All(it => it == '=');
        }

        private static string NormalizeEntryName(string entryName)
        {
            return (entryName ?? string.Empty).Replace('\\', '/').TrimStart('/');
        }

        private class ItemBuilder
        {
            public string Sequence { get; set; }

            public string PackageVersion { get; set; }

            public List<string> ContentLines { get; set; } = new List<string>();
        }
    }
}
