using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Patchwork;

namespace LobotomyBaseModPatcher
{
    public class Copper
    {
        // Token: 0x06000005 RID: 5 RVA: 0x00002238 File Offset: 0x00000438
        public Copper()
        {
            Console.WriteLine("패치 파일 복사 중...");
            FileInfo basemod = new FileInfo(Environment.CurrentDirectory + "/Assembly-CSharp.dll");
            File.Copy(basemod.FullName, Program.Managed.FullName + "/Assembly-CSharp.dll", true);
            FileInfo harmony = new FileInfo(Environment.CurrentDirectory + "/0Harmony.dll");
            File.Copy(harmony.FullName, Program.Managed.FullName + "/0Harmony.dll", true);
            FileInfo lobo = new FileInfo(Environment.CurrentDirectory + "/LobotomyBaseModLib.dll");
            File.Copy(lobo.FullName, Program.Managed.FullName + "/LobotomyBaseModLib.dll", true);
            FileInfo naudio = new FileInfo(Environment.CurrentDirectory + "/NAudio.dll");
            File.Copy(naudio.FullName, Program.Managed.FullName + "/NAudio.dll", true);
            Console.WriteLine("패치 파일 복사 완료");
        }
    }
    public class Folder
    {
        // Token: 0x06000004 RID: 4 RVA: 0x000021B8 File Offset: 0x000003B8
        public Folder()
        {
            Console.WriteLine("BaseMods 폴더 확인 중...");
            bool flag = !Directory.Exists(Program.Managed.Parent.FullName + "/BaseMods");
            if (flag)
            {
                Console.WriteLine("모드 폴더가 확인되지 않았습니다. 해당 폴더를 생성합니다.");
                Directory.CreateDirectory(Program.Managed.Parent.FullName + "/BaseMods");
            }
            else
            {
                Console.WriteLine("모드 폴더가 이미 존재합니다. 이 단계를 스킵합니다.");
            }
        }
    }
    public class Patcher
    {
        // Token: 0x06000006 RID: 6 RVA: 0x000022D0 File Offset: 0x000004D0
        public Patcher()
        {
            Console.WriteLine("스크립트 파일 패치 중...");
            string path = Program.Managed.FullName + "/Assembly-CSharp.dll";
            AssemblyPatcher patcher = new AssemblyPatcher(path, null);
            patcher.PatchAssembly(Environment.CurrentDirectory + "/Lobotomypatch.dll", null, true);
            patcher.WriteTo(Program.Managed.FullName + "/Assembly-CSharp.dll");
            Console.WriteLine("패치 성공! 프로그램을 종료해주세요.");
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000234C File Offset: 0x0000054C
        public static void MakeOpenAssembly(AssemblyDefinition assembly, bool modifyEvents)
        {
            IEnumerable<TypeDefinition> enumerable = assembly.MainModule.GetAllTypes();
            enumerable = enumerable.ToList<TypeDefinition>();
            foreach (TypeDefinition typeDefinition in enumerable)
            {
                foreach (FieldDefinition fieldDefinition in typeDefinition.Fields)
                {
                    fieldDefinition.IsPublic = true;
                    fieldDefinition.IsInitOnly = false;
                }
                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                {
                    methodDefinition.IsPublic = true;
                }
                if (modifyEvents)
                {
                    using (var enumerator4 = typeDefinition.Events.GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            EventDefinition vent = enumerator4.Current;
                            bool flag = typeDefinition.Fields.Any((FieldDefinition x) => x.Name == vent.Name) || typeDefinition.Properties.Any((PropertyDefinition x) => x.Name == vent.Name);
                            bool flag2 = flag;
                            if (flag2)
                            {
                                EventDefinition vent2 = vent;
                                EventDefinition eventDefinition = vent2;
                                eventDefinition.Name += "Event";
                            }
                        }
                    }
                }
                typeDefinition.IsSealed = false;
                bool isNested = typeDefinition.IsNested;
                bool flag3 = isNested;
                if (flag3)
                {
                    typeDefinition.IsNestedPublic = true;
                }
                else
                {
                    typeDefinition.IsPublic = true;
                }
            }
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("패치 현 버전 : basemod 1.21v - LoR 1.0.3.1");
                DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
                Console.WriteLine("현재 위치 : " + Environment.CurrentDirectory);
                DirectoryInfo startdir = dir.Parent;
                Program.Managed = null;
                Program.FindPath(startdir);
                bool flag = Program.Managed == null || !Program.Managed.Exists;
                if (flag)
                {
                    Console.WriteLine("패치 경로를 찾지 못했습니다.프로그램을 종료해주세요");
                }
                new Copper();
                new Folder();
                Program.patcher = new Patcher();
            }
            catch (Exception e)
            {
                Console.WriteLine("에러 : " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            while(true)
            {

            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002120 File Offset: 0x00000320
        public static void FindPath(DirectoryInfo curdir)
        {
            bool flag = File.Exists(curdir.FullName + "/Assembly-CSharp.dll") && curdir.Name == "Managed" && Program.Managed == null;
            if (flag)
            {
                Program.Managed = curdir;
                Console.WriteLine("패치 파일 위치 : " + Program.Managed.FullName);
            }
            else
            {
                foreach (DirectoryInfo dir in curdir.GetDirectories())
                {
                    Program.FindPath(dir);
                }
            }
        }

        // Token: 0x04000001 RID: 1
        public static DirectoryInfo Managed;

        // Token: 0x04000002 RID: 2
        public static Patcher patcher;
    }
}
