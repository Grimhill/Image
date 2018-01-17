using System;
using System.Collections.Generic;
using System.Linq;

//padArray operations
namespace Image
{
    public class PadMyArray<T>
    {
        //padR and padC both cant be 0 in one time, coz there is no sence 
        //call
        //padMyArray<double> padArr;
        //padArr = new padMyArray<double>();

        public T[,] PadArrayByNum(T[,] arr, int padR, int padC, int num, Direction direction)
        {
            int width = arr.GetLength(1);
            int height = arr.GetLength(0);

            List<T> arrlits = arr.Cast<T>().ToList();
            List<T> temp = new List<T>();
            List<T> newarrList = new List<T>();
            List<T> numRepeat = new List<T>();

            T[,] prePost = new T[height + padR, width + padC];
            T[,] Both = new T[height + 2 * padR, width + 2 * padC];
            T[,] result;

            switch (direction.ToString())
            {
                case "pre":     //add num on left and\or top side                
                    if (padR == 0)
                    {
                        temp = arrlits;
                        //add num on left side 
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 0; i < temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }
                    }
                    else if (padC == 0)
                    {
                        newarrList = arrlits;
                        //add num on top side
                        numRepeat = new List<T>(Enumerable.Repeat((T)(object)num, width).ToArray()); //Enumerable.Repeat(x, n).ToArray();
                        for (int i = 0; i < padR; i++)
                        {
                            newarrList.InsertRange(0, numRepeat);
                        }

                    }
                    else
                    {
                        temp = arrlits;
                        //add num on left side 
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 0; i < temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }

                        //add num on top side
                        numRepeat = new List<T>(Enumerable.Repeat((T)(object)num, width + padC).ToArray()); //Enumerable.Repeat(x, n).ToArray();
                        for (int i = 0; i < padR; i++)
                        {
                            newarrList.InsertRange(0, numRepeat);
                        }
                    }
                    break;

                case "post":   //add num on right and\or bot side
                    if (padR == 0)
                    {
                        temp = arrlits;
                        //add num on right side
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 1; i <= temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }
                    }
                    else if (padC == 0)
                    {
                        newarrList = arrlits;
                        //add num on bot side
                        numRepeat = new List<T>(Enumerable.Repeat((T)(object)num, width).ToArray());
                        for (int i = 0; i < padR; i++)
                        {
                            newarrList.AddRange(numRepeat);
                        }
                    }
                    else
                    {
                        temp = arrlits;
                        //add num on right side
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 1; i <= temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }

                        //add num on bot side
                        numRepeat = new List<T>(Enumerable.Repeat((T)(object)num, width + padC).ToArray());
                        for (int i = 0; i < padR; i++)
                        {
                            newarrList.AddRange(numRepeat);
                        }
                    }
                    break;

                case "both":  //add num on left, right and\or top and bot side
                    if (padR == 0) //add num on left and right
                    {
                        //add num on left side   
                        temp = arrlits;
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 0; i < temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }

                        //add num on right side
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 1; i <= temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1 + padC) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }
                    }
                    else if (padC == 0)//add num on top and bot
                    {
                        newarrList = arrlits;
                        numRepeat = new List<T>(Enumerable.Repeat((T)(object)num, width).ToArray());
                        for (int i = 0; i < padR; i++)
                        {
                            newarrList.InsertRange(0, numRepeat);
                            newarrList.AddRange(numRepeat);
                        }
                    }
                    else //add num at each side
                    {
                        //add zeros on left side   
                        temp = arrlits;
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 0; i < temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }

                        //add zeros on left side
                        for (int k = 0; k < padC; k++)
                        {
                            newarrList = new List<T>();
                            int j = 0;
                            for (int i = 1; i <= temp.Count + height; i++)
                            {
                                newarrList.Add(i % (width + k + 1 + padC) == 0 ? (T)(object)num : temp[j++]);
                            }
                            temp = newarrList;
                        }

                        numRepeat = new List<T>(Enumerable.Repeat((T)(object)num, width + 2 * padC).ToArray());
                        for (int i = 0; i < padR; i++)
                        {
                            newarrList.InsertRange(0, numRepeat);
                            newarrList.AddRange(numRepeat);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (direction.ToString() == "both")
            {
                //convert result form list into array
                int c = 0;
                for (int u = 0; u < Both.GetLength(0); u++)
                {
                    for (int b = 0; b < Both.GetLength(1); b++)
                    {
                        Both[u, b] = newarrList[c];
                        c++;
                    }
                }
                result = Both;
            }
            else if (direction.ToString() == "pre" || direction.ToString() == "post")
            {
                //convert result form list into array
                int c = 0;
                for (int u = 0; u < prePost.GetLength(0); u++)
                {
                    for (int b = 0; b < prePost.GetLength(1); b++)
                    {
                        prePost[u, b] = newarrList[c];
                        c++;
                    }
                }
                result = prePost;
            }
            else { result = arr; }

            return result;
        }

        public T[,] PadArray(T[,] arr, int padR, int padC) //pad array by zeros both
        {
            //default direction = both
            //default type = pad by zeros

            int width = arr.GetLength(1);
            int height = arr.GetLength(0);

            List<T> arrlits = arr.Cast<T>().ToList();
            List<T> temp = new List<T>();
            List<T> newarrList = new List<T>();

            T[,] result = new T[height + 2 * padR, width + 2 * padC];

            //add left, right and\or top, bot zeros around
            if (padR == 0) //add zeros on left and right side
            {
                //add zeros on left side   
                temp = arrlits;
                for (int k = 0; k < padC; k++)
                {
                    newarrList = new List<T>();
                    int j = 0;
                    for (int i = 0; i < temp.Count + height; i++)
                    {
                        //Convert.ChangeType(PlayerStats[type], typeof(T))
                        newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)0 : temp[j++]);
                    }
                    temp = newarrList;
                }

                //add zeros on right side
                for (int k = 0; k < padC; k++)
                {
                    newarrList = new List<T>();
                    int j = 0;
                    for (int i = 1; i <= temp.Count + height; i++)
                    {
                        newarrList.Add(i % (width + k + 1 + padC) == 0 ? (T)(object)0 : temp[j++]);
                    }
                    temp = newarrList;
                }
            }
            else if (padC == 0) //add zeros on top and bot side
            {
                newarrList = arrlits;
                var x = new List<T>(new T[width]);
                for (int i = 0; i < padR; i++)
                {
                    newarrList.InsertRange(0, x);
                    newarrList.AddRange(x);
                }
            }
            else //add zeros on each side
            {
                //add zeros on left side   
                temp = arrlits;
                for (int k = 0; k < padC; k++)
                {
                    newarrList = new List<T>();
                    int j = 0;
                    for (int i = 0; i < temp.Count + height; i++)
                    {
                        newarrList.Add(i % (width + k + 1) == 0 ? (T)(object)0 : temp[j++]);
                    }
                    temp = newarrList;
                }

                //add zeros on left side
                for (int k = 0; k < padC; k++)
                {
                    newarrList = new List<T>();
                    int j = 0;
                    for (int i = 1; i <= temp.Count + height; i++)
                    {
                        newarrList.Add(i % (width + k + 1 + padC) == 0 ? (T)(object)0 : temp[j++]);
                    }
                    temp = newarrList;
                }

                var x = new List<T>(new T[width + 2 * padC]);
                for (int i = 0; i < padR; i++)
                {
                    newarrList.InsertRange(0, x);
                    newarrList.AddRange(x);
                }
            }

            //convert result form list into array
            int c = 0;
            for (int u = 0; u < result.GetLength(0); u++)
            {
                for (int b = 0; b < result.GetLength(1); b++)
                {
                    result[u, b] = newarrList[c];
                    c++;
                }
            }

            return result;
        }

        //
        /*public T[,] padArray(T[,] arr, int padR, int padC, string type)
        {
            //default direction - both            
        }*/

        public T[,] PadArray(T[,] arr, int padR, int padC, PadType padType, Direction direct) //BDSM array function
        {
            //type - symmetric, replicate, zeros
            //direction - pre, post, both
            int width = arr.GetLength(1);
            int height = arr.GetLength(0);

            //padding helpers
            List<T> b_l = new List<T>(); //left
            List<T> b_r = new List<T>(); //right
            List<T> b_t = new List<T>(); //top
            List<T> b_b = new List<T>(); //bottom  

            //pre & post
            T[,] temp_cols = new T[height, width + padC]; //temp for cols
            T[,] temp = new T[temp_cols.GetLength(0) + padR, temp_cols.GetLength(1)];
            //both
            T[,] temp_colsBoth = new T[height, width + 2 * padC]; //temp for cols   
            T[,] tempBoth = new T[temp_colsBoth.GetLength(0) + 2 * padR, temp_colsBoth.GetLength(1)];

            T[,] result;
            string type = padType.ToString();
            string direction = direct.ToString();

            if ((padC > width || padR > height) & (type == "symmetric" || type == "circular" || type == "circmirror")) //later make only for symmetric ? i don`t need symmetric more, than whole array
            {
                //not implemented symmentic more, than whole array
                Console.WriteLine("Bro, i can`t in this operation, if pad width or height more than image have");
            }
            else
            {
                switch (direction)
                {
                    case "pre": //add left and\or top border\mirror\circular\zeros
                        if (padR == 0) //add left border\mirror\circular\zeros cols
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain left image border
                                    for (int k = 0; k < height; k++)
                                    {
                                        b_l.Add(arr[k, 0]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain left cols for mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain right cols for circular
                                    for (int l = padC; l > 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain right cols for circular mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //left zeros
                                    b_l = new List<T>(new T[height]);
                                    break;
                                default:
                                    b_l = new List<T>(new T[height]);
                                    break;
                            }

                            var l_mc = 0;
                            //add left border\mirror\circular\zeros cols
                            for (int row = 0; row < height; row++)
                            {
                                l_mc = row;
                                for (int col = 0; col < width + padC; col++)
                                {
                                    if (col < padC)
                                    {
                                        temp_cols[row, col] = b_l[l_mc];
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            l_mc = l_mc + height;
                                        }
                                    }
                                    else
                                    {
                                        temp_cols[row, col] = arr[row, col - padC];
                                    }
                                }
                            }

                            temp = temp_cols;
                        }
                        else if (padC == 0) //add top border\mirror\circular\zeros rows
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain top image border        
                                    for (int k = 0; k < width; k++)
                                    {
                                        b_t.Add(arr[0, k]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain top rows for mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_t.Add(arr[l, k]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain bot rows for circular
                                    for (int l = padR; l > 0; l--)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_t.Add(arr[height - l, k]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain bot rows for circular mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_t.Add(arr[height - l, k]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //top zeros
                                    b_t = new List<T>(new T[width]);
                                    break;
                                default:
                                    b_t = new List<T>(new T[width]);
                                    break;
                            }

                            var t_mr = 0;
                            //add top top border\mirror\circular\zeros rows
                            for (int row = 0; row < height + padR; row++)
                            {
                                for (int col = 0; col < width; col++)
                                {
                                    if (row < padR)
                                    {
                                        temp[row, col] = b_t[t_mr];
                                        t_mr++;
                                    }
                                    else
                                    {
                                        temp[row, col] = arr[row - padR, col];
                                    }
                                }
                                if (type == "replicate" || type == "zeros")
                                {
                                    t_mr = 0;
                                }
                            }
                        }
                        else //add left and top border\mirror\circular\zeros
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain left image border
                                    for (int m = 0; m < height; m++)
                                    {
                                        b_l.Add(arr[m, 0]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain left cols for mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain right cols for circular
                                    for (int l = padC; l > 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain right cols for circular mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //left zeros
                                    b_l = new List<T>(new T[height]);
                                    break;
                                default:
                                    b_l = new List<T>(new T[height]);
                                    break;
                            }

                            var l_mc = 0;
                            //add left border\mirror\zeros cols
                            for (int row = 0; row < height; row++)
                            {
                                l_mc = row;
                                for (int col = 0; col < width + padC; col++)
                                {
                                    if (col < padC)
                                    {
                                        temp_cols[row, col] = b_l[l_mc];
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            l_mc = l_mc + height;
                                        }
                                    }
                                    else
                                    {
                                        temp_cols[row, col] = arr[row, col - padC];
                                    }
                                }
                            }

                            switch (type)
                            {
                                case "replicate":
                                    //obtain top image border        
                                    for (int k = 0; k < width + padC; k++)
                                    {
                                        b_t.Add(temp_cols[0, k]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain top rows for mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width + padC; k++)
                                        {
                                            b_t.Add(temp_cols[l, k]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain bot rows for circular
                                    for (int l = padR; l > 0; l--)
                                    {
                                        for (int k = 0; k < width + padC; k++)
                                        {
                                            b_t.Add(temp_cols[height - l, k]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain bot rows for circular mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width + padC; k++)
                                        {
                                            b_t.Add(temp_cols[height - l, k]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //top zeros
                                    b_t = new List<T>(new T[width + padC]);
                                    break;
                                default:
                                    b_t = new List<T>(new T[width + padC]);
                                    break;
                            }

                            var t_mr = 0;
                            //add top border\mirror\circular\zeros rows
                            for (int row = 0; row < height + padR; row++)
                            {
                                for (int col = 0; col < width + padC; col++)
                                {
                                    if (row < padR)
                                    {
                                        temp[row, col] = b_t[t_mr];
                                        t_mr++;
                                    }
                                    else
                                    {
                                        temp[row, col] = temp_cols[row - padR, col];
                                    }
                                }
                                if (type == "replicate" || type == "zeros")
                                {
                                    t_mr = 0;
                                }
                            }
                        }
                        break;

                    case "post":  //add right and\or bot border\mirror\circular\zeros                            
                        if (padR == 0) //add right border\mirror\circular\zeros cols
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain right image border
                                    for (int k = 0; k < height; k++)
                                    {
                                        b_r.Add(arr[k, width - 1]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain right cols for mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain left cols for circular
                                    for (int l = 0; l < padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain left cols for circular mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //right zeros
                                    b_r = new List<T>(new T[height]);
                                    break;
                                default:
                                    b_r = new List<T>(new T[height]);
                                    break;
                            }

                            var r_mc = 0;
                            //add right border\mirror\zeros cols
                            for (int row = 0; row < height; row++)
                            {
                                r_mc = row;
                                for (int col = 0; col < width + padC; col++)
                                {
                                    if (col > width - 1)
                                    {
                                        temp_cols[row, col] = b_r[r_mc];
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            r_mc = r_mc + height;
                                        }
                                    }
                                    else
                                    {
                                        temp_cols[row, col] = arr[row, col];
                                    }
                                }
                            }

                            temp = temp_cols;
                        }
                        else if (padC == 0) //add bot border\mirror\circular\zeros rows
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain bottom image border
                                    for (int k = 0; k < width; k++)
                                    {
                                        b_b.Add(arr[height - 1, k]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain bot rows for mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_b.Add(arr[height - l, k]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain top rows for circular
                                    for (int l = 0; l < padR; l++)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_b.Add(arr[l, k]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain top rows for circular mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_b.Add(arr[l, k]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //bot zeros
                                    b_b = new List<T>(new T[width]);
                                    break;
                                default:
                                    b_b = new List<T>(new T[width]);
                                    break;
                            }

                            var b_mr = 0;
                            //add bot border\mirror\circular\zeros rows
                            for (int row = 0; row < height + padR; row++)
                            {
                                for (int col = 0; col < width; col++)
                                {
                                    if (row > height - 1)
                                    {
                                        temp[row, col] = b_b[b_mr];
                                        b_mr++;
                                    }
                                    else
                                    {
                                        temp[row, col] = arr[row, col];
                                    }
                                }
                                if (type == "replicate" || type == "zeros")
                                {
                                    b_mr = 0;
                                }
                            }
                        }
                        else //add right and bot border\mirror\circular\zeros
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain right image border
                                    for (int k = 0; k < height; k++)
                                    {
                                        b_r.Add(arr[k, width - 1]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain right cols for mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain left cols for circular
                                    for (int l = 0; l < padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain left cols for circular mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //right zeros
                                    b_r = new List<T>(new T[height]);
                                    break;
                                default:
                                    b_r = new List<T>(new T[height]);
                                    break;
                            }

                            var r_mc = 0;
                            //add right border\mirror\circular\zeros cols
                            for (int row = 0; row < height; row++)
                            {
                                r_mc = row;
                                for (int col = 0; col < width + padC; col++)
                                {
                                    if (col > width - 1)
                                    {
                                        temp_cols[row, col] = b_r[r_mc];
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            r_mc = r_mc + height;
                                        }
                                    }
                                    else
                                    {
                                        temp_cols[row, col] = arr[row, col];
                                    }
                                }
                            }

                            switch (type)
                            {
                                case "replicate":
                                    //obtain bottom image border
                                    for (int k = 0; k < width + padC; k++)
                                    {
                                        b_b.Add(temp_cols[height - 1, k]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain bot rows for mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width + padC; k++)
                                        {
                                            b_b.Add(temp_cols[height - l, k]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain top rows for circular
                                    for (int l = 0; l < padR; l++)
                                    {
                                        for (int k = 0; k < width + padC; k++)
                                        {
                                            b_b.Add(temp_cols[l, k]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain top rows for circular mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width + padC; k++)
                                        {
                                            b_b.Add(temp_cols[l, k]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //bot zeros
                                    b_b = new List<T>(new T[width + padC]);
                                    break;
                                default:
                                    b_b = new List<T>(new T[width + padC]);
                                    break;
                            }

                            var b_mr = 0;
                            //add bot mirror
                            for (int row = 0; row < height + padR; row++)
                            {
                                for (int col = 0; col < width + padC; col++)
                                {
                                    if (row > height - 1)
                                    {
                                        temp[row, col] = b_b[b_mr];
                                        b_mr++;
                                    }
                                    else
                                    {
                                        temp[row, col] = temp_cols[row, col];
                                    }
                                }
                                if (type == "replicate" || type == "zeros")
                                {
                                    b_mr = 0;
                                }
                            }
                        }
                        break;

                    case "both":  //add right, left and\or top, bot border\mirror\circular\zeros 
                        if (padR == 0) //add left and right border\mirror\circular\zeros cols
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain left image border
                                    for (int m = 0; m < height; m++)
                                    {
                                        b_l.Add(arr[m, 0]);
                                    }

                                    //obtain right image border
                                    for (int n = 0; n < height; n++)
                                    {
                                        b_r.Add(arr[n, width - 1]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain left cols for mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, l]);
                                        }
                                    }

                                    //obtain right cols for mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain right cols for circular
                                    for (int l = padC; l > 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }

                                    //obtain left cols for circular
                                    for (int l = 0; l < padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain right cols for circular mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }

                                    //obtain left cols for circular mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //left zeros
                                    b_l = new List<T>(new T[height]);

                                    //right zeros
                                    b_r = new List<T>(new T[height]);
                                    break;
                                default:
                                    b_l = new List<T>(new T[height]);

                                    b_r = new List<T>(new T[height]);
                                    break;
                            }

                            var l_mc = 0;
                            var r_mc = 0;
                            //add left and right border\mirror\circular\zeros cols
                            for (int row = 0; row < height; row++)
                            {
                                l_mc = row;
                                r_mc = row;
                                for (int col = 0; col < width + 2 * padC; col++)
                                {
                                    if (col < padC)
                                    {
                                        temp_colsBoth[row, col] = b_l[l_mc]; //add left
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            l_mc = l_mc + height;
                                        }
                                    }
                                    else if (col > width + padC - 1)
                                    {
                                        temp_colsBoth[row, col] = b_r[r_mc]; //add right
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            r_mc = r_mc + height;
                                        }
                                    }
                                    else
                                    {
                                        temp_colsBoth[row, col] = arr[row, col - padC];
                                    }
                                }
                            }

                            tempBoth = temp_colsBoth;
                        }
                        else if (padC == 0) //add top and bot border\mirror\zeros rows
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain top image border        
                                    for (int m = 0; m < width; m++)
                                    {
                                        b_t.Add(arr[0, m]);
                                    }

                                    //obtain bottom image border
                                    for (int n = 0; n < width; n++)
                                    {
                                        b_b.Add(arr[height - 1, n]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain top rows for mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_t.Add(arr[l, k]);
                                        }
                                    }

                                    //obtain bot rows for mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_b.Add(arr[height - l, k]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain bot rows for circular
                                    for (int l = padR; l > 0; l--)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_t.Add(arr[height - l, k]);
                                        }
                                    }

                                    //obtain top rows for circular
                                    for (int l = 0; l < padR; l++)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_b.Add(arr[l, k]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain bot rows for circular mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_t.Add(arr[height - l, k]);
                                        }
                                    }

                                    //obtain top rows for circular mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            b_b.Add(arr[l, k]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //top zeros
                                    b_t = new List<T>(new T[width]);

                                    //bot zeros
                                    b_b = new List<T>(new T[width]);
                                    break;
                                default:
                                    b_t = new List<T>(new T[width]);

                                    b_b = new List<T>(new T[width]);
                                    break;
                            }

                            var t_mr = 0;
                            var b_mr = 0;
                            //add top and bot border\mirror\circular\zeros rows
                            for (int row = 0; row < height + 2 * padR; row++)
                            {
                                for (int col = 0; col < width; col++)
                                {
                                    if (row < padR)
                                    {
                                        tempBoth[row, col] = b_t[t_mr]; //add top
                                        t_mr++;
                                    }
                                    else if (row > height + padR - 1)
                                    {
                                        tempBoth[row, col] = b_b[b_mr]; //add bot
                                        b_mr++;
                                    }
                                    else
                                    {
                                        tempBoth[row, col] = arr[row - padR, col];
                                    }
                                }
                                if (type == "replicate" || type == "zeros")
                                {
                                    t_mr = 0;
                                    b_mr = 0;
                                }
                            }
                        }
                        else //add all border\mirror\circular\zeros
                        {
                            switch (type)
                            {
                                case "replicate":
                                    //obtain left image border
                                    for (int m = 0; m < height; m++)
                                    {
                                        b_l.Add(arr[m, 0]);
                                    }

                                    //obtain right image border
                                    for (int n = 0; n < height; n++)
                                    {
                                        b_r.Add(arr[n, width - 1]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain left cols for mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, l]);
                                        }
                                    }

                                    //obtain right cols for mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, width - l]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain right cols for circular
                                    for (int l = padC; l > 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }

                                    //obtain left cols for circular
                                    for (int l = 0; l < padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain right cols for circular mirror
                                    for (int l = 1; l <= padC; l++)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_l.Add(arr[k, width - l]);
                                        }
                                    }

                                    //obtain left cols for circular mirror
                                    for (int l = padC - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < height; k++)
                                        {
                                            b_r.Add(arr[k, l]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //left zeros
                                    b_l = new List<T>(new T[height]);

                                    //right zeros
                                    b_r = new List<T>(new T[height]);
                                    break;
                                default:
                                    b_l = new List<T>(new T[height]);

                                    b_r = new List<T>(new T[height]);
                                    break;
                            }

                            var l_mc = 0;
                            var r_mc = 0;
                            //add left and right border\mirror\circular\zeros cols
                            for (int row = 0; row < height; row++)
                            {
                                l_mc = row;
                                r_mc = row;
                                for (int col = 0; col < width + 2 * padC; col++)
                                {
                                    if (col < padC)
                                    {
                                        temp_colsBoth[row, col] = b_l[l_mc]; //add left
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            l_mc = l_mc + height;
                                        }
                                    }
                                    else if (col > width + padC - 1)
                                    {
                                        temp_colsBoth[row, col] = b_r[r_mc]; //add right
                                        if (type == "symmetric" || type == "circular" || type == "circmirror")
                                        {
                                            r_mc = r_mc + height;
                                        }
                                    }
                                    else
                                    {
                                        temp_colsBoth[row, col] = arr[row, col - padC];
                                    }
                                }
                            }

                            switch (type)
                            {
                                case "replicate":
                                    //obtain top image border        
                                    for (int m = 0; m < width + 2 * padC; m++)
                                    {
                                        b_t.Add(temp_colsBoth[0, m]);
                                    }

                                    //obtain bottom image border
                                    for (int n = 0; n < width + 2 * padC; n++)
                                    {
                                        b_b.Add(temp_colsBoth[height - 1, n]);
                                    }
                                    break;
                                case "symmetric":
                                    //obtain top rows for mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width + 2 * padC; k++)
                                        {
                                            b_t.Add(temp_colsBoth[l, k]);
                                        }
                                    }

                                    //obtain bot rows for mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width + 2 * padC; k++)
                                        {
                                            b_b.Add(temp_colsBoth[height - l, k]);
                                        }
                                    }
                                    break;
                                case "circular":
                                    //obtain bot rows for circular
                                    for (int l = padR; l > 0; l--)
                                    {
                                        for (int k = 0; k < width + 2 * padC; k++)
                                        {
                                            b_t.Add(temp_colsBoth[height - l, k]);
                                        }
                                    }

                                    //obtain top rows for circular
                                    for (int l = 0; l < padR; l++)
                                    {
                                        for (int k = 0; k < width + 2 * padC; k++)
                                        {
                                            b_b.Add(temp_colsBoth[l, k]);
                                        }
                                    }
                                    break;
                                case "circmirror":
                                    //obtain bot rows for circular mirror
                                    for (int l = 1; l <= padR; l++)
                                    {
                                        for (int k = 0; k < width + 2 * padC; k++)
                                        {
                                            b_t.Add(temp_colsBoth[height - l, k]);
                                        }
                                    }

                                    //obtain top rows for circular mirror
                                    for (int l = padR - 1; l >= 0; l--)
                                    {
                                        for (int k = 0; k < width + 2 * padC; k++)
                                        {
                                            b_b.Add(temp_colsBoth[l, k]);
                                        }
                                    }
                                    break;
                                case "zeros":
                                    //top zeros
                                    b_t = new List<T>(new T[width + 2 * padC]);

                                    //bot zeros
                                    b_b = new List<T>(new T[width + 2 * padC]);
                                    break;
                                default:
                                    b_t = new List<T>(new T[width + 2 * padC]);

                                    b_b = new List<T>(new T[width + 2 * padC]);
                                    break;
                            }

                            var t_mr = 0;
                            var b_mr = 0;
                            //add top and bot border\mirror\circular\zeros rows
                            for (int row = 0; row < height + 2 * padR; row++)
                            {
                                for (int col = 0; col < width + 2 * padC; col++)
                                {
                                    if (row < padR)
                                    {
                                        tempBoth[row, col] = b_t[t_mr]; //add top
                                        t_mr++;
                                    }
                                    else if (row > height + padR - 1)
                                    {
                                        tempBoth[row, col] = b_b[b_mr]; //add bot
                                        b_mr++;
                                    }
                                    else
                                    {
                                        tempBoth[row, col] = temp_colsBoth[row - padR, col];
                                    }
                                }
                                if (type == "replicate" || type == "zeros")
                                {
                                    t_mr = 0;
                                    b_mr = 0;
                                }
                            }
                        }
                        break;
                    default:
                        result = arr;
                        break;
                }
            }
            if (direction == "both")
            { result = tempBoth; }
            else if (direction == "pre" || direction == "post") { result = temp; }
            else { result = arr; }

            return result;
        }
    }

    public enum PadType
    {
        replicate,
        symmetric,
        circular,
        circmirror,
        zeros
    }

    public enum Direction
    {
        pre,
        post,
        both
    }
}
