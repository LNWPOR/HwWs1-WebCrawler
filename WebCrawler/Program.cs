using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

public class MyQueue
{
    ArrayList q;
    public MyQueue()
    {
        q = new ArrayList();
    }
    public void enQueue(string url)
    {
        q.Add(url);
    }
    public string deQueue()
    {
        string res = "";
        if (q.Count > 0)
        {
            res += q[0];
            q.RemoveAt(0); // BFS
            // res += q[q.Count-1];
            // q.RemoveAt[q.Count-1]; // DFS
        }
        return res;
    }
    public void insertAt(int index, string url)
    {
        q.Insert(index, url);
    }
    public int Count()
    {
        return q.Count;
    }
    public bool Contains(string url)
    {
        return q.Contains(url);
    }
}

public class MyClass
{
    public static int count = 0;
    public static List<string> chiefList = new List<string>();
    public static List<string> thaiSubjectList = new List<string>();
    public static bool linkFound = false;
    public static string linkPathFile = @"D:\1path.txt";
    public static string chiefFile = @"D:\25_Chief.txt";
    public static string thaiSubjectsFile = @"D:\24thai_subjects.txt";
    public static string reportErrorPageFile = @"D:\21_reportErrorPage.txt";
    public static string websitesDirPath = @"D:\websites";
    public static string previousURL = "";

    static void _CCW(ConsoleColor c, string t, int i)
    {
        ConsoleColor orig = Console.ForegroundColor;
        Console.ForegroundColor = c;
        if (i == 1)
            Console.WriteLine(t);
        else
            Console.Write(t);
        Console.ForegroundColor = orig;
    }

    static void CCWL(ConsoleColor c, string t)
    {
        _CCW(c, t, 1);
    }

    static void CCW(ConsoleColor c, string t)
    {
        _CCW(c, t, 0);
    }

    public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public static string getPage(string url, int nbPage)
    {
        //WebRequest.DefaultWebProxy = new WebProxy("http://yourproxy.com:3128");
        WebRequest req = WebRequest.Create(url);
        ((HttpWebRequest)req).UserAgent = "204453 Spider written by Punnatad Chansri, id5610500231";
        req.Timeout = 1000; // 1000ms
        // handle https
        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        // print some message
        Console.Write("[{0}] ", nbPage);
        CCW(ConsoleColor.Green, "Downloading>> ");
        CCWL(ConsoleColor.White, url);
        // receive response from target url
        WebResponse resp = req.GetResponse();
        // get response stream (data)
        Stream st = resp.GetResponseStream();
        // create streamreader pbject to read the data
        StreamReader sr = new StreamReader(st);
        string page = sr.ReadToEnd();
        sr.Close();
        resp.Close();
        return page;

    }

    public static string getRobot(string url)
    {
        //WebRequest.DefaultWebProxy = new WebProxy("http://yourproxy.com:3128");
        WebRequest req = WebRequest.Create(url);
        ((HttpWebRequest)req).UserAgent = "204453 Spider written by Punnatad Chansri, id5610500231";
        req.Timeout = 1000; // 1000ms
        // handle https
        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        // receive response from target url
        WebResponse resp = req.GetResponse();
        // get response stream (data)
        Stream st = resp.GetResponseStream();
        // create streamreader pbject to read the data
        StreamReader sr = new StreamReader(st);
        string page = sr.ReadToEnd();
        sr.Close();
        resp.Close();
        return page;
    }

    public static Dictionary<string, string> getHeader(string url, string filePath)
    {
        Dictionary<string, string> header = new Dictionary<string, string>();
        try
        {
            // create WebRequest
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            // handle https
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            // create response object
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                foreach (string key in resp.Headers)
                {
                    // add header to the dico
                    header.Add(key, resp.Headers[key]);
                    //Console.WriteLine("{0} : {1}", key, header[key]);
                }
                header["status_code"] = "OK";
            }
            // close WebRequest
            resp.Close();
        }
        catch (WebException e)
        {
            reportPageError(url, e);
        }
        // return header
        return header;
    }

    static void parseHtml(string page, MyQueue FQ, MyQueue VQ)
    {
        string HREF = "<a href=\"";
        int HREFL = HREF.Length;
        page = page.ToLower();
        int start = 0, end = 0, url_length = 0;
        string url = "";
        while (page.IndexOf(HREF, start) >= 0)
        {
            start = page.IndexOf(HREF, start) + HREFL;
            end = page.IndexOf("\"", start);
            url_length = end - start;
            url = page.Substring(start, url_length);
            if (url.IndexOf("http://") >= 0)
                //Console.WriteLine("{0} --{1}-{2}--{3}", url, start, end, url_length);
                if (!FQ.Contains(url) && !VQ.Contains(url) && url.ToLower().IndexOf("ku.ac.th/") >= 0)
                {
                    FQ.enQueue(url);
                    CCW(ConsoleColor.Green, " Inserting ");
                    Console.Write("{0}", url);

                    //write path
                    if (previousURL != "")
                    {
                        writeFile(previousURL + " ---> " + url, linkPathFile);
                    }

                    CCWL(ConsoleColor.Green, " into frontierQ");
                }
            //Console.ReadLine();
            start = end;
            //System.Threading.Thread.Sleep(5);
        }
    }

    static void writeFile(string text, string filePath)
    {
        try
        {
            StreamWriter s = new StreamWriter(filePath, true);
            s.WriteLine(text);
            s.Close();
        }
        catch (IOException e)
        {
            Console.WriteLine("IO Error");
        }
    }

    static void findThaiSubjects(string url, string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("หลักสูตร"))
        {
            if (page.Contains("หลักสูตรปริญญาตรีภาคปกติ"))
            {
                //CCWL(ConsoleColor.Red, "หลักสูตรปริญญาตรีภาคปกติ");
                writeFile("หลักสูตรปริญญาตรีภาคปกติ", thaiSubjectsFile);
                if (!linkFound)
                {
                    linkFound = true;
                    for (int subjectPage_id = 11346; subjectPage_id <= 11370; subjectPage_id += 2)
                    {
                        string HREF;
                        HREF = "<li><a href=\"http://www.eng.ku.ac.th/?page_id=" + subjectPage_id + "\">";
                        int HREFL;
                        HREFL = HREF.Length;
                        page = page.ToLower();
                        int start = 0, end = 0, url_length = 0, countFind = 0;
                        string subjectName;

                        while (page.IndexOf(HREF, start) >= 0 && countFind <= 1)
                        {
                            start = page.IndexOf(HREF, start) + HREFL;
                            end = page.IndexOf("</a></li>", start);
                            url_length = end - start;
                            subjectName = page.Substring(start, url_length);

                            countFind++;
                            //CCWL(ConsoleColor.Red, subjectName);
                            thaiSubjectList.Add(subjectName);
                            writeFile(subjectName, thaiSubjectsFile);

                            start = end;
                            System.Threading.Thread.Sleep(5);
                        }
                    }
                }
            }
            if (page.Contains("หลักสูตรปริญญาตรีภาคพิเศษ"))
            {
                writeFile("\nหลักสูตรปริญญาตรีภาคพิเศษ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาตรีภาคพิเศษ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;

                    writeFile(subjects.Replace("<li>", "").Replace("</li>", ""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }
            if (page.Contains("หลักสูตรปริญญาตรีนานาชาติ"))
            {
                writeFile("\nหลักสูตรปริญญาตรีนานาชาติ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาตรีนานาชาติ</strong></p>\n<ul>\n<li><a href=\"http://iup.eng.ku.ac.th/\" target=\"_blank\">";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;

                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", ""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }

            if (page.Contains("หลักสูตรปริญญาโทภาคปกติ"))
            {
                writeFile("\nหลักสูตรปริญญาโทภาคปกติ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาโทภาคปกติ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;
                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", "").Replace("<a href=\"http://ie.eng.ku.ac.th/master.html\" target=\"_blank\">",""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }
            if (page.Contains("หลักสูตรปริญญาโทภาคพิเศษ"))
            {
                writeFile("\nหลักสูตรปริญญาโทภาคพิเศษ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาโทภาคพิเศษ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;
                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7efengsup/\" target=\"_blank\">","")
                        .Replace("<a href=\"http://safety.eng.ku.ac.th/\" target=\"_blank\">","")
                        .Replace("<a href=\"http://pe.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.meipt.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://emp.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.mfpe.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.stbe.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://cpeg.cpe.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7Efengsup/\" target=\"_blank\">",""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }
            if (page.Contains("หลักสูตรปริญญาโทนานาชาติ"))
            {
                writeFile("\nหลักสูตรปริญญาโทนานาชาติ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาโทนานาชาติ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;
                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", "")
                        .Replace("<a href=\"http://ieinter.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7efengsup/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://ieinter.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://ceinter.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7Efengsup/\" target=\"_blank\">", ""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }

            if (page.Contains("หลักสูตรปริญญาเอกภาคปกติ"))
            {
                writeFile("\nหลักสูตรปริญญาเอกภาคปกติ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาเอกภาคปกติ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;
                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", "")
                        .Replace("<a href=\"http://ie.eng.ku.ac.th/doctor.html\" target=\"_blank\">", ""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }
            if (page.Contains("หลักสูตรปริญญาเอกภาคพิเศษ"))
            {
                writeFile("\nหลักสูตรปริญญาเอกภาคพิเศษ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาเอกภาคพิเศษ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;
                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", "")
                        .Replace("<a href=\"http://ieinter.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://emp.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7efengsup/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://cpeg.cpe.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7Efengsup/\" target=\"_blank\">", ""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }
            if (page.Contains("หลักสูตรปริญญาเอกนานาชาติ"))
            {
                writeFile("\nหลักสูตรปริญญาเอกนานาชาติ", thaiSubjectsFile);
                string HREF;
                HREF = "<p><strong>หลักสูตรปริญญาเอกนานาชาติ</strong></p>\n<ul>";
                int HREFL;
                HREFL = HREF.Length;
                int start = 0, end = 0, url_length = 0, countFind = 0;
                string subjects;
                while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
                {
                    start = page.IndexOf(HREF, start) + HREFL;
                    end = page.IndexOf("</ul>", start);
                    url_length = end - start;
                    subjects = page.Substring(start, url_length);
                    countFind++;
                    writeFile(subjects.Replace("<li>", "").Replace("</li>", "").Replace("</a>", "")
                        .Replace("<a href=\"http://ieinter.eng.ku.ac.th/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7efengsup/\" target=\"_blank\">", "")
                        .Replace("<a href=\"http://www.pirun.ku.ac.th/%7Efengsup/\" target=\"_blank\">", ""), thaiSubjectsFile);
                    start = end;
                    System.Threading.Thread.Sleep(5);
                }
            }

        }
    }

    static void reportPageError(string url, WebException e)
    {
        try
        {
            StreamWriter s = new StreamWriter(reportErrorPageFile, true);

            Console.WriteLine(url);
            s.WriteLine(url);
            Console.WriteLine("Exception Message :" + e.Message);
            s.WriteLine("Exception Message :" + e.Message);
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
                Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                s.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                s.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                s.WriteLine("\n");

                count++;
            }
            System.Threading.Thread.Sleep(5000);
            s.Close();
        }
        catch (IOException E)
        {
            Console.WriteLine("IO Error");
        }
    }

    static void deleteExistFile(string pathFile)
    {
        if (File.Exists(pathFile))
        {
            File.Delete(pathFile);
        }
    }

    static void initFile(string pathFile)
    {
        deleteExistFile(pathFile);
    }

    static void saveWeb(string url, string page)
    {
        Console.WriteLine(websitesDirPath + "\\" + url);
        string websiteFileName = url.Replace(":", ".").Replace("\\", ".").Replace("/", ".").Replace("?", ".");
        //also add url tag for indexing
        writeFile("<url>" + url + "</url>\n" + page, websitesDirPath + "\\" + websiteFileName + ".html");
    }

    static string checkPage(string page, string startPos, string endPos)
    {
        string HREF;
        HREF = startPos;
        int HREFL;
        HREFL = HREF.Length;
        page = page.ToLower();
        int start = 0, end = 0, url_length = 0, countFind = 0;
        string textFind = "";

        while (page.IndexOf(HREF, start) >= 0 && countFind <= 1)
        {
            start = page.IndexOf(HREF, start) + HREFL;
            end = page.IndexOf(endPos, start);
            url_length = end - start;
            textFind = page.Substring(start, url_length);
        }

        return textFind;
    }

    static void findASE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("ภาควิชาวิศวกรรมการบินและอวกาศ คณะวิศวกรรมศาสตร์ มหาวิทยาลัยเกษตรศาสตร์ - ผู้บริหารภาควิชา"))
        {
            string HREF;
            HREF = "hreflang=\"th\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                
                string saveText = "ภาควิชาวิศวกรรมการบินและอวกาศ : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findEE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("ภาควิชาวิศวกรรมไฟฟ้า คณะวิศวกรรมศาสตร์ มหาวิทยาลัยเกษตรศาสตร์ - วชิระ จงบุรี, ผศ."))
        {
            string HREF;
            HREF = "<strong>";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมไฟฟ้า : " + chiefName;
                checkChiefList(saveText);


                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findIE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("คณาจารย์ - ภาควิชาวิศวกรรมอุตสาหการ คณะวิศวกรรมศาสตร์ มหาวิทยาลัยเกษตรศาสตร์"))
        {
            string HREF;
            HREF = "<div class=\"staff_name style62 style90 style96\"><strong>";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf(" (หัวหน้าภาควิชาวิศวกรรมอุตสาหการ)", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมอุตสาหการ : " + chiefName;
                checkChiefList(saveText);


                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findME(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("ฝ่ายบริหารและคณาจารย์ - ภาควิชาวิศวกรรมเครื่องกล	คณะวิศวกรรมศาสตร์, มหาวิทยาลัยเกษตรศาสตร์"))
        {
            string HREF;
            HREF = "<a href=\"/index.php/th/faculties-staffs-2/administrations-staffs/53-lecturer/224-prapot.html\" hreflang=\"th\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมเครื่องกล : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findWRE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("ภาควิชาวิศวกรรมทรัพยากรน้ำ คณะวิศวกรรมศาสตร์ มหาวิทยาลัยเกษตรศาสตร์ - ผู้บริหารภาควิชา"))
        {
            string HREF;
            HREF = "<h2 class=\"art-postheader\"><a href=\"/index.php/th/tea/198-jirawat.html\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("(", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมทรัพยากรน้ำ : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findEVE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("ภาควิชาวิศวกรรมสิ่งแวดล้อม คณะวิศวกรรมศาสตร์ มหาวิทยาลัยเกษตรศาสตร์ - หน้าแรก"))
        {
            string HREF;
            HREF = "<h2 class=\"art-postheader\"><a href=\"/index.php/th/tea/190-suchat2\" class=\"postheader\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("(", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมสิ่งแวดล้อม : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findCHE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
            .Equals("home"))
        {

            string HREF;
            HREF = "<img src=\"/images/sia.jpg\" border=\"0\" style=\"border: 0; float: left;\" /><a href=\"http://pirun.ku.ac.th/~fengsia/\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมเคมี : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }
    static void findCPESKE(string page)
    {
        if (checkPage(page, "รศ.ดร.อนันต์ ", "เพิ่ม")
                      .Equals("ผล"))
        {
            Console.WriteLine("gg");
            string HREF;
            HREF = "39w\" sizes=\"(max-width: 90px) 100vw, 90px\" /></td>\n<td style=\"text-align: center;\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวิศวกรรมคอมพิวเตอร์ : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }
    static void findMAT(string page)
    {
        if (checkPage(page, "assoc.prof.", " polsilapa")
                      .Equals("sureerat"))
        {
            Console.WriteLine("gg");
            string HREF;
            HREF = "uploadFile/user_pic/img_10327269_10203423473091000_2011182093_n.jpg\" style=\" margin-bottom:10px;\" /></div><div class=\"c78r\" align=\"left\"><div class=\"subcolumns\"><div class=\"c35l\" align=\"left\"><h3 style=\" padding-top:10px;\">";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาวัสดุ : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void findCE(string page)
    {
        if (checkPage(page, "<title>", "</title>")
                      .Equals("ภาควิชาวิศวกรรมโยธา คณะวิศวกรรมศาสตร์ มหาวิทยาลัยเกษตรศาสตร์   &raquo; ดร.วันชัย ยอดสุดใจ"))
        {
            string HREF;
            HREF = "&raquo; ";
            int HREFL;
            HREFL = HREF.Length;
            int start = 0, end = 0, url_length = 0, countFind = 0;
            string chiefName;
            while (page.IndexOf(HREF, start) >= 0 && countFind < 1)
            {
                start = page.IndexOf(HREF, start) + HREFL;
                end = page.IndexOf("<", start);
                url_length = end - start;
                chiefName = page.Substring(start, url_length);
                countFind++;

                string saveText = "ภาควิชาโยธา : " + chiefName;
                checkChiefList(saveText);

                start = end;
                System.Threading.Thread.Sleep(5);
            }
        }
    }

    static void checkChiefList(string saveText)
    {
        if (!chiefList.Contains(saveText))
        {
            CCWL(ConsoleColor.Red, saveText);
            writeFile(saveText, chiefFile);
            chiefList.Add(saveText);
        }
    }

    static void Main(String[] argh)
    {
        initFile(linkPathFile);
        initFile(thaiSubjectsFile);
        initFile(chiefFile);
        initFile(reportErrorPageFile);

        string url = "https://mike.cpe.ku.ac.th/seed";
        //string url = "http://mike.cpe.ku.ac.th/01204453";
        //string url = "https://developer.android.com/preview/setup-sdk.html#java8";
        //string url = "http://www.eng.ku.ac.th/?page_id=9690";

        //string url = "http://ase.eng.ku.ac.th/index.php?option=com_content&view=article&id=25&Itemid=132&lang=th";
        //string url = "http://www.ee.ku.ac.th/index.php?option=com_content&view=article&id=113:wachira&catid=22&Itemid=206&lang=th";
        //string url = "http://ie.eng.ku.ac.th/index.php/staff/lecturer.html";
        //string url = "http://www.me.eng.ku.ac.th/index.php/th/faculties-staffs-2/administrations-staffs.html"; 
        //string url = "http://wre.eng.ku.ac.th/index.php/th/2013-03-26-23-32-14/administrative.html";
        //string url = "http://www.che.eng.ku.ac.th/index.php/en/";
        //string url = "https://www.cpe.ku.ac.th/?page_id=58";
        //string url = "http://mat.eng.ku.ac.th/people.php?menuIds=5&id=38&contentId=33";
        //string url = "http://www.ce.eng.ku.ac.th/บุคลากร/คณาจารย์/โครงสร้าง/วันชัย-ยอดสุดใจ/";

        //string url = "http://www.eng.ku.ac.th/?page_id=9690";

        // your variables here
        Dictionary<string, string> header;
        MyQueue visitedQ = new MyQueue();
        MyQueue frontierQ = new MyQueue();

        frontierQ.enQueue(url);
        string page = "";
        int nbPage = 0, MAXPAGE = 1000;

        writeFile("1:ข้อมูลแสดงความสัมพันธ์การเชื่อมโยงกันของเว็บเพจ", linkPathFile);
        writeFile("2.1:Error Page Log", reportErrorPageFile);
        writeFile("2.4:Thai Subjects", thaiSubjectsFile);
        writeFile("2.5:Chief", chiefFile);

        int limitLoad = 5;
        int limitCount = 0;
        while (frontierQ.Count() > 0 && nbPage < MAXPAGE)
        {
            try
            {
            
                url = frontierQ.deQueue();
                //CCWL(ConsoleColor.Red, url.ToString());
                //CCWL(ConsoleColor.Red, previousURL.ToString());
                string currentDomainName = "";
                string previousDomainName = "";
                bool haveRobot = false;
                bool canDownload = false;
                if (previousURL != "")
                {
                    
                    int endCurrentDomainIndex = url.IndexOf(".ku.ac.th/");
                    for (int i = 0; i < endCurrentDomainIndex; i++)
                    {
                        currentDomainName += url[i];
                        //CCW(ConsoleColor.Red, currentDomainName[i].ToString());
                    }
                    //Console.WriteLine();
                    int endPreviousDomainIndex = previousURL.IndexOf(".ku.ac.th/");
                    for (int i = 0; i < endPreviousDomainIndex; i++)
                    {
                        previousDomainName += previousURL[i];
                        //CCW(ConsoleColor.Red, previousDomainName[i].ToString());
                    }
                    //CCWL(ConsoleColor.Red, currentDomainName + ".ku.ac.th/robots.txt");
                    //CCWL(ConsoleColor.Red, getPage(currentDomainName + ".ku.ac.th/robots.txt",0));
                    //CCWL(ConsoleColor.Red, getPage("https://www.google.co.th/robots.txt", 0));
                    try
                    {
                        if (getRobot(currentDomainName + ".ku.ac.th/robots.txt") != null)
                        {
                            haveRobot = true;
                        }
                    }
                    catch
                    {
                        //CCWL(ConsoleColor.Red, "gg");
                    }
                    
                }

                //check robots.txt
                if (haveRobot)
                {
                    if (getRobot(currentDomainName + ".ku.ac.th/robots.txt").Contains("User-agent: *"))
                    {
                        canDownload = true;
                    }else
                    {
                        canDownload = false;
                    }
                }else
                {
                    canDownload = true;
                }

                if (previousURL.Equals("") || !currentDomainName.Equals(previousDomainName) || limitCount > limitLoad && canDownload)
                {
                    if (limitCount > limitLoad)
                    {
                        limitCount = 0;
                    }

                    visitedQ.enQueue(url);
                    header = getHeader(url, reportErrorPageFile);
                    if (header["status_code"].IndexOf("OK") >= 0 && header["Content-Type"].IndexOf("text/html") >= 0)
                    {
                        Console.WriteLine();
                        page = getPage(url, ++nbPage);

                        parseHtml(page, frontierQ, visitedQ);
                        previousURL = url;

                        findThaiSubjects(url, page);
                        //findASE(page);
                        //findEE(page);
                        //findIE(page);
                        //findME(page);
                        //findWRE(page);
                        //findEVE(page);
                        //findCHE(page);
                        //findMAT(page);
                        //findCE(page);
                        //findCPESKE(page);

                        saveWeb(url, page);
                        //System.Threading.Thread.Sleep(100);
                        Console.WriteLine("{0} - {1}", frontierQ.Count(), visitedQ.Count());
                    }
                    if (nbPage % 100 == 0)
                        System.Threading.Thread.Sleep(3000);
                }else
                {
                    //CCWL(ConsoleColor.Red, currentDomainName.ToString());
                    //CCWL(ConsoleColor.Red, previousDomainName.ToString());
                    //CCWL(ConsoleColor.Red, limitCount.ToString());
                    limitCount += 1;
                    //frontierQ.enQueue(url);
                    frontierQ.insertAt(limitLoad, url);
                }

                
            }
            catch (Exception e)
            {
                //CCW(ConsoleColor.Red, "--> Exception: ");
                //CCWL(ConsoleColor.Yellow, e.Message.ToString() + " [" + url + "]");
            }
        }

        //2.1
        Console.WriteLine("2.1:Error Page Total = {0}", count);
        writeFile("Error Page Total : " + count, reportErrorPageFile);
        Console.WriteLine();

        //2.4
        Console.WriteLine("2.4:Thai Subjects");
        foreach (string thaiSubject in thaiSubjectList)
        {
            Console.WriteLine(thaiSubject);
        }

        //2.5
        Console.WriteLine("2.5:Chief");
        foreach (string chief in chiefList)
        {
            Console.WriteLine(chief);
        }

        Console.Read();
    }
}