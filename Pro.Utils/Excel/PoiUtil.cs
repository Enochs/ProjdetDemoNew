using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pro.Utils
{

    //参见 https://my.oschina.net/psuyun/blog/157990

    /**
* POI工具类 功能点： 
* 1、实现excel的sheet复制，复制的内容包括单元的内容、样式、注释
* 2、setMForeColor修改HSSFColor.YELLOW的色值，setMBorderColor修改PINK的色值
* 
* @author Administrator
*/
    public sealed class PoiUtil
    {

        /**
         * 功能：拷贝sheet
         * 实际调用 	copySheet(targetSheet, sourceSheet, targetWork, sourceWork, true)
         * @param targetSheet
         * @param sourceSheet
         * @param targetWork
         * @param sourceWork                                                                   
         */
        public static void copySheet(HSSFSheet targetSheet, HSSFSheet sourceSheet,
                HSSFWorkbook targetWork, HSSFWorkbook sourceWork)
        {
            if (targetSheet == null || sourceSheet == null || targetWork == null || sourceWork == null)
            {
                throw new Exception("调用PoiUtil.copySheet()方法时，targetSheet、sourceSheet、targetWork、sourceWork都不能为空，故抛出该异常！");
            }
            copySheet(targetSheet, sourceSheet, targetWork, sourceWork, true);
        }

        /**
         * 功能：拷贝sheet
         * @param targetSheet
         * @param sourceSheet
         * @param targetWork
         * @param sourceWork
         * @param copyStyle					boolean 是否拷贝样式
         */
        public static void copySheet(HSSFSheet targetSheet, HSSFSheet sourceSheet,
                HSSFWorkbook targetWork, HSSFWorkbook sourceWork, bool copyStyle)
        {

            if (targetSheet == null || sourceSheet == null || targetWork == null || sourceWork == null)
            {
                throw new Exception("调用PoiUtil.copySheet()方法时，targetSheet、sourceSheet、targetWork、sourceWork都不能为空，故抛出该异常！");
            }

            //复制源表中的行
            int maxColumnNum = 0;

            Hashtable styleMap = (copyStyle) ? new Hashtable() : null;


            HSSFPatriarch patriarch = (HSSFPatriarch)targetSheet.CreateDrawingPatriarch(); //用于复制注释
            for (int i = sourceSheet.FirstRowNum; i <= sourceSheet.LastRowNum; i++)
            {
                HSSFRow sourceRow = (HSSFRow)sourceSheet.GetRow(i);
                HSSFRow targetRow = (HSSFRow)targetSheet.CreateRow(i);

                if (sourceRow != null)
                {
                    copyRow(targetRow, sourceRow,
                            targetWork, sourceWork, patriarch, styleMap);
                    if (sourceRow.LastCellNum > maxColumnNum)
                    {
                        maxColumnNum = sourceRow.LastCellNum;
                    }
                }
            }

            //复制源表中的合并单元格
            mergerRegion(targetSheet, sourceSheet);

            //设置目标sheet的列宽
            for (int i = 0; i <= maxColumnNum; i++)
            {
                targetSheet.SetColumnWidth(i, sourceSheet.GetColumnWidth(i));
            }
        }

        /**
         * 功能：拷贝row
         * @param targetRow
         * @param sourceRow
         * @param styleMap
         * @param targetWork
         * @param sourceWork
         * @param targetPatriarch
         */
        public static void copyRow(HSSFRow targetRow, HSSFRow sourceRow,
                HSSFWorkbook targetWork, HSSFWorkbook sourceWork, HSSFPatriarch targetPatriarch, Hashtable styleMap)
        {
            if (targetRow == null || sourceRow == null || targetWork == null || sourceWork == null || targetPatriarch == null)
            {
                throw new Exception("调用PoiUtil.copyRow()方法时，targetRow、sourceRow、targetWork、sourceWork、targetPatriarch都不能为空，故抛出该异常！");
            }

            //设置行高
            targetRow.Height = sourceRow.Height;//失效

            for (int i = sourceRow.FirstCellNum; i <= sourceRow.LastCellNum; i++)
            {
                HSSFCell sourceCell = (HSSFCell)sourceRow.GetCell(i);
                HSSFCell targetCell = (HSSFCell)targetRow.GetCell(i);

                if (sourceCell != null)
                {
                    if (targetCell == null)
                    {
                        targetCell = (HSSFCell)targetRow.CreateCell(i);
                    }

                    //拷贝单元格，包括内容和样式
                    copyCell(targetCell, sourceCell, targetWork, sourceWork, styleMap);

                    //拷贝单元格注释
                    copyComment(targetCell, sourceCell, targetPatriarch);
                }
            }
        }

        /**
         * 功能：拷贝cell，依据styleMap是否为空判断是否拷贝单元格样式
         * @param targetCell			不能为空
         * @param sourceCell			不能为空
         * @param targetWork			不能为空
         * @param sourceWork			不能为空
         * @param styleMap				可以为空				
         */
        public static void copyCell(HSSFCell targetCell, HSSFCell sourceCell, HSSFWorkbook targetWork, HSSFWorkbook sourceWork, Hashtable styleMap)
        {
            if (targetCell == null || sourceCell == null || targetWork == null || sourceWork == null)
            {
                throw new Exception("调用PoiUtil.copyCell()方法时，targetCell、sourceCell、targetWork、sourceWork都不能为空，故抛出该异常！");
            }

            //处理单元格样式
            if (styleMap != null)
            {
                if (targetWork == sourceWork)
                {

                    targetCell.SetCellType((CellType)sourceCell.CellType);
                }
                else
                {
                    String stHashCode = "" + sourceCell.CellStyle.GetHashCode();
                    HSSFCellStyle targetCellStyle = (HSSFCellStyle)styleMap[stHashCode];
                    if (targetCellStyle == null)
                    {
                        targetCellStyle = (HSSFCellStyle)targetWork.CreateCellStyle();
                        targetCellStyle.CloneStyleFrom(sourceCell.CellStyle);
                        styleMap.Add(stHashCode, targetCellStyle);
                    }
                    targetCell.CellStyle = targetCellStyle;

                }
            }

            //处理单元格内容
            switch (sourceCell.CellType)
            {
                case CellType.String:
                    targetCell.SetCellValue(sourceCell.RichStringCellValue);
                    break;
                case CellType.Numeric:
                    targetCell.SetCellValue(sourceCell.NumericCellValue);
                    break;
                case CellType.Blank:
                    targetCell.SetCellType(CellType.Blank);
                    break;
                case CellType.Boolean:
                    targetCell.SetCellValue(sourceCell.BooleanCellValue);
                    break;
                case CellType.Error:
                    targetCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                    break;
                case CellType.Formula:
                    targetCell.SetCellFormula(sourceCell.CellFormula);
                    break;
                default:
                    break;
            }
        }

        /**
         * 功能：拷贝comment
         * @param targetCell
         * @param sourceCell
         * @param targetPatriarch
         */
        public static void copyComment(HSSFCell targetCell, HSSFCell sourceCell, HSSFPatriarch targetPatriarch)
        {
            if (targetCell == null || sourceCell == null || targetPatriarch == null)
            {
                throw new Exception("调用PoiUtil.copyCommentr()方法时，targetCell、sourceCell、targetPatriarch都不能为空，故抛出该异常！");
            }

            //处理单元格注释
            HSSFComment comment = (HSSFComment)sourceCell.CellComment;
            if (comment != null)
            {
                HSSFComment newComment = targetPatriarch.CreateComment(new HSSFClientAnchor());
                //newComment.Anchor =comment.Author;
                //newComment.setAuthor(comment.getAuthor());
                //newComment.setColumn(comment.getColumn());
                //newComment.setFillColor(comment.getFillColor());
                //newComment.setHorizontalAlignment(comment.getHorizontalAlignment());
                //newComment.setLineStyle(comment.getLineStyle());
                //newComment.setLineStyleColor(comment.getLineStyleColor());
                //newComment.setLineWidth(comment.getLineWidth());
                //newComment.setMarginBottom(comment.getMarginBottom());
                //newComment.setMarginLeft(comment.getMarginLeft());
                //newComment.setMarginTop(comment.getMarginTop());
                //newComment.setMarginRight(comment.getMarginRight());
                //newComment.setNoFill(comment.isNoFill());
                //newComment.setRow(comment.getRow());
                //newComment.setShapeType(comment.getShapeType());
                //newComment.setString(comment.getString());
                //newComment.setVerticalAlignment(comment.getVerticalAlignment());
                //newComment.setVisible(comment.isVisible());
                //targetCell.setCellComment(newComment);
            }
        }

        /**
         * 功能：复制原有sheet的合并单元格到新创建的sheet
         * 
         * @param sheetCreat
         * @param sourceSheet
         */
        public static void mergerRegion(HSSFSheet targetSheet, HSSFSheet sourceSheet)
        {
            if (targetSheet == null || sourceSheet == null)
            {
                throw new Exception("调用PoiUtil.mergerRegion()方法时，targetSheet或者sourceSheet不能为空，故抛出该异常！");
            }

            for (int i = 0; i < sourceSheet.NumMergedRegions; i++)
            {
                CellRangeAddress oldRange = sourceSheet.GetMergedRegion(i);
                CellRangeAddress newRange = new CellRangeAddress(
                        oldRange.FirstRow, oldRange.LastRow,
                        oldRange.FirstColumn, oldRange.LastColumn);
                targetSheet.AddMergedRegion(newRange);
            }
        }


    }

}
