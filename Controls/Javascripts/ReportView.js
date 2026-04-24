var errorMessage = "";

// ************************************ Enums *********************************
var enumItemType = {
    DataField: 1,
    FormulaField: 2,
    Section: 3,
    Label: 4,
    PageNumber: 5,
    PageTotal: 6,
    PageNofM: 7,
    ReportTitle: 8,
    ReportUser: 9,  //First Last
    //ReportUserFML: 10, //First Middle Last
    //ReportUserLF: 11,  //Last First
    //ReportUserLFM: 12, //Last First Middle
    //ReportUserF: 13, //First
    //ReportUserM: 14, //Middle
    //ReportUserL: 15, //Last
    PrintDate: 10,
    PrintTime: 11,
    PrintDateTime: 12,
    Query: 13,
    ConfidentialityStatement: 14,
    ReportComments: 15,
    Image: 16 , //Path to Image
    Group: 17
}

var enumReportTemplate = {
    Tabular: 1,
    FreeForm: 2
}
var enumFieldLayout = {
    Inline: 1,
    Block: 2,
    None: 3
}
var enumMessageType = {
    Error: 1,
    Warning: 2,
    Information: 3,
    None: 4
};

// ************************************ ReportView ****************************
function ReportView()
{
    this.reportView = {
        ReportID: '',
        ReportTemplate: enumReportTemplate.Tabular,
        Title: '',
        Orientation: 'portrait',
        ReportFieldLayout: 'Block',
        HeaderHeight: '1',
        HeaderBackColor: 'white',
        HeaderBorderColor: 'lightgrey',
        HeaderBorderStyle: 'Solid',
        HeaderBorderWidth: '1',
        FooterHeight: '1',
        FooterBackColor: 'white',
        FooterBorderColor: 'lightgrey',
        FooterBorderStyle: 'Solid',
        FooterBorderWidth: '1',
        DataFontName: 'Tahoma',
        DataFontSize: '12px',
        DataForeColor: 'black',
        DataBackColor: 'white',
        DataBorderColor: 'lightgrey',
        DataBorderStyle: 'Solid',
        DataBorderWidth: '1',
        DataFontStyle: 'Regular',
        DataUnderline: false,
        DataStrikeout: false,
        ReportDetailAlign: 'Left',
        LabelFontName: 'Tahoma',
        LabelFontSize: '12px',
        LabelForeColor: 'black',
        LabelBackColor: 'white',
        LabelBorderColor: 'lightgrey',
        LabelBorderStyle: 'Solid',
        LabelBorderWidth: '1',
        LabelFontStyle: 'Regular',
        LabelUnderline: false,
        LabelStrikeout: false,
        ReportCaptionAlign: 'Left',
        MarginBottom: '0.5',
        MarginLeft: '0.25',
        MarginRight: '0.25',
        MarginTop: '0.5',
        Items: [],
        _init: function() {
            this.Items.AddAt = function (index, item) {

                if (item == void 0) {
                    //alert('Item cannot be null.');
                    showMessage("Item can not be null.","Alert")
                    return null;
                }
                if (index.toString().indexOf('-') >= 0) {
                    //alert('Index cannot be negative.');
                    showMessage("Index cannot be negative.","Alert")
                    return null;
                };
                if (index > this.length)
                    index = this.length;
                this.splice(index, 0, item);
                return item;
            };
            this.Items.Add = function (item) {
                return this.AddAt(this.length, item);
            };
            this.Items.GetItem = function (itemID) {

                for (var index = 0; index < this.length; index++) {
                    var _item = this[index];
                    if (_item.ItemID == itemID)
                        return _item;
                }
                return null;
            };
            this.Items.Remove = function (item) {
                if (item == void 0) {
                    showMessage("Item can not be null.", "Alert")
                    return null;
                }
                if (this.length > 0) {
                    var index = 0;

                    for (index = 0; index <= this.length - 1; index++) {
                        var _item = this[index];

                        if (item == _item) {
                            this.splice(index, 1);
                            break;
                        };
                    };
                };
                if (this.length > 0) {
                    for (index = 0; index <= this.length - 1; index++) {
                        this[index].ItemOrder = (index + 1)
                    }
                }
            }
            this.Items.RemoveAt = function (index) {
                if (index == void 0) {
                    showMessage("Item can not be null.", "Alert")
                    return null;
                }
                if (index <= this.length - 1) {
                    var item = this[index];
                    this.Remove(item);
                }
            }
            this.Items.Clear = function () {
                while (this.length > 0)
                    this.RemoveAt(0);
            }
        },
        ToJSON: function () {
            var __reportView = {
                ReportID: this.ReportID || "",
                ReportTemplate: "",
                Title: this.Title || "",
                Orientation: this.Orientation || 'portrait',
                ReportFieldLayout: this.ReportFieldLayout || 'Block',
                HeaderHeight: this.HeaderHeight || '1',
                HeaderBackColor: this.HeaderBackColor || 'white',
                HeaderBorderColor: this.HeaderBorderColor || 'lightgrey',
                HeaderBorderStyle: this.HeaderBorderStyle || 'Solid',
                HeaderBorderWidth: this.HeaderBorderWidth || '1',
                FooterHeight: this.FooterHeight || '1',
                FooterBackColor: this.FooterBackColor || 'white',
                FooterBorderColor: this.FooterBorderColor || 'lightgrey',
                FooterBorderStyle: this.FooterBorderStyle || 'Solid',
                FooterBorderWidth: this.FooterBorderWidth || '1',
                DataFontName: this.DataFontName || 'Tahoma',
                DataFontSize: this.DataFontSize || '12px',
                DataForeColor: this.DataForeColor || 'black',
                DataBackColor: this.DataBackColor || 'white',
                DataBorderColor: this.DataBorderColor || 'lightgrey',
                DataBorderStyle: this.DataBorderStyle || 'Solid',
                DataBorderWidth: this.DataBorderWidth || '1',

                DataFontStyle: this.DataFontStyle || 'Regular',
                DataUnderline: this.DataUnderline || false,
                DataStrikeout: this.DataStrikeout || false,
                ReportDetailAlign: this.ReportDetailAlign || 'Left',

                LabelFontName: this.LabelFontName || 'Tahoma',
                LabelFontSize: this.LabelFontSize || '12px',
                LabelForeColor: this.LabelForeColor || 'black',
                LabelBackColor: this.LabelBackColor || 'white',
                LabelBorderColor: this.LabelBorderColor || 'lightgrey',
                LabelBorderStyle: this.LabelBorderStyle || 'Solid',
                LabelBorderWidth: this.LabelBorderWidth || '1',

                LabelFontStyle: this.LabelFontStyle || 'Regular',
                LabelUnderline: this.LabelUnderline || false,
                LabelStrikeout: this.LabelStrikeout || false,
                ReportCaptionAlign: this.ReportCaptionAlign || 'Left',

                MarginBottom: this.MarginBottom || '0.5',
                MarginLeft: this.MarginLeft || '0.25',
                MarginRight: this.MarginRight || '0.25',
                MarginTop: this.MarginTop || '0.5',

                Items: null
            };
            if (this.ReportTemplate == enumReportTemplate.FreeForm)
                __reportView.ReportTemplate = "FreeForm";
            else
                __reportView.ReportTemplate = "Tabular";
            if (this.Items.length > 0) {
                __reportView.Items = [];
                for (var index=0; index<=this.Items.length-1; index++)
                    __reportView.Items.push(this.Items[index].ToJSON())
            }
            return __reportView;
        }
    }
    this.reportView._init();
    return this.reportView;
}

ReportView.ParseJSON = function (json, reportView) {
    reportView = reportView || new ReportView();

    if (json.ReportID != void 0 && typeof (json.ReportID) == "string" && json.ReportID.length > 0)
        reportView.ReportID = json.ReportID;

    if (json.ReportTemplate != void 0 && typeof (json.ReportTemplate) == "string" && json.ReportTemplate.length > 0) {
        if (json.ReportTemplate.toLowerCase() == "freeform")
            reportView.ReportTemplate = enumReportTemplate.FreeForm;
        else
            reportView.ReportTemplate = enumReportTemplate.Tabular;
    }
    else
        reportView.ReportTemplate = enumReportTemplate.Tabular;

    if (json.Title!= void 0 && typeof (json.Title) == "string" && json.Title.length > 0)
        reportView.Title = json.Title;
    if (json.Orientation!= void 0 && typeof (json.Orientation) == "string" && json.Orientation.length > 0)
        reportView.Orientation = json.Orientation;
    if (json.ReportFieldLayout!= void 0 && typeof (json.ReportFieldLayout) == "string" && json.ReportFieldLayout.length > 0)
        reportView.ReportFieldLayout = json.ReportFieldLayout;
    //
    if (json.HeaderHeight!= void 0 && typeof (json.HeaderHeight) == "string" && json.HeaderHeight.length > 0)
        reportView.HeaderHeight = json.HeaderHeight;
    if (json.HeaderBackColor!= void 0 && typeof (json.HeaderBackColor) == "string" && json.HeaderBackColor.length > 0)
        reportView.HeaderBackColor = json.HeaderBackColor;
    if (json.HeaderBorderColor!= void 0 && typeof (json.HeaderBorderColor) == "string" && json.HeaderBorderColor.length > 0)
        reportView.HeaderBorderColor = json.HeaderBorderColor;
    if (json.HeaderBorderStyle!= void 0 && typeof (json.HeaderBorderStyle) == "string" && json.HeaderBorderStyle.length > 0)
        reportView.HeaderBorderStyle = json.HeaderBorderStyle;
    if (json.HeaderBorderWidth!= void 0 && typeof (json.HeaderBorderWidth) == "string" && json.HeaderBorderWidth.length > 0)
        reportView.HeaderBorderWidth= json.HeaderBorderWidth;
    //
    if (json.FooterHeight!= void 0 && typeof (json.FooterHeight) == "string" && json.FooterHeight.length > 0)
        reportView.FooterHeight = json.FooterHeight;
    if (json.FooterBackColor!= void 0 && typeof (json.FooterBackColor) == "string" && json.FooterBackColor.length > 0)
        reportView.FooterBackColor = json.FooterBackColor;
    if (json.FooterBorderColor!= void 0 && typeof (json.FooterBorderColor) == "string" && json.FooterBorderColor.length > 0)
        reportView.FooterBorderColor = json.FooterBorderColor;
    if (json.FooterBorderStyle!= void 0 && typeof (json.FooterBorderStyle) == "string" && json.FooterBorderStyle.length > 0)
        reportView.FooterBorderStyle = json.FooterBorderStyle;
    if (json.FooterBorderWidth!= void 0 && typeof (json.FooterBorderWidth) == "string" && json.FooterBorderWidth.length > 0)
        reportView.FooterBorderWidth= json.FooterBorderWidth;
    //
    if (json.DataFontName != void 0 && typeof (json.DataFontName) == "string" && json.DataFontName.length > 0)
        reportView.DataFontName = json.DataFontName;
    if (json.DataFontSize != void 0 && typeof (json.DataFontSize) == "string" && json.DataFontSize.length > 0)
        reportView.DataFontSize = json.DataFontSize;
    if (json.DataForeColor != void 0 && typeof (json.DataForeColor) == "string" && json.DataForeColor.length > 0)
        reportView.DataForeColor = json.DataForeColor;
    if (json.DataBackColor != void 0 && typeof (json.DataBackColor) == "string" && json.DataBackColor.length > 0)
        reportView.DataBackColor = json.DataBackColor;
    if (json.DataBorderColor != void 0 && typeof (json.DataBorderColor) == "string" && json.DataBorderColor.length > 0)
        reportView.DataBorderColor = json.DataBorderColor;
    if (json.DataBorderStyle != void 0 && typeof (json.DataBorderStyle) == "string" && json.DataBorderStyle.length > 0)
        reportView.DataBorderStyle = json.DataBorderStyle
    if (json.DataBorderWidth != void 0 && typeof (json.DataBorderWidth) == "string" && json.DataBorderWidth.length > 0)
        reportView.DataBorderWidth = json.DataBorderWidth
    if (json.DataFontStyle != void 0 && typeof (json.DataFontStyle) == "string" && json.DataFontStyle.length > 0)
        reportView.DataFontStyle = json.DataFontStyle;
    if (json.DataUnderline != void 0 && typeof (json.DataUnderline) == "boolean")
        reportView.DataUnderline = json.DataUnderline;
    if (json.DataStrikeout != void 0 && typeof (json.DataStrikeout) == "boolean")
        reportView.DataStrikeout = json.DataStrikeout;
    if (json.ReportDetailAlign != void 0 && typeof (json.ReportDetailAlign) == "string" && json.ReportDetailAlign.length > 0)
        reportView.ReportDetailAlign = json.ReportDetailAlign;
    if (json.LabelFontName != void 0 && typeof (json.LabelFontName) == "string" && json.LabelFontName.length > 0)
        reportView.LabelFontName = json.LabelFontName;
    if (json.LabelFontSize != void 0 && typeof (json.LabelFontSize) == "string" && json.LabelFontSize.length > 0)
        reportView.LabelFontSize = json.LabelFontSize;
    if (json.LabelForeColor != void 0 && typeof (json.LabelForeColor) == "string" && json.LabelForeColor.length > 0)
        reportView.LabelForeColor = json.LabelForeColor;
    if (json.LabelBackColor != void 0 && typeof (json.LabelBackColor) == "string" && json.LabelBackColor.length > 0)
        reportView.LabelBackColor = json.LabelBackColor;
    if (json.LabelBorderColor != void 0 && typeof (json.LabelBorderColor) == "string" && json.LabelBorderColor.length > 0)
        reportView.LabelBorderColor = json.LabelBorderColor;
    if (json.LabelBorderStyle != void 0 && typeof (json.LabelBorderStyle) == "string" && json.LabelBorderStyle.length > 0)
        reportView.LabelBorderStyle = json.LabelBorderStyle
    if (json.LabelBorderWidth != void 0 && typeof (json.LabelBorderWidth) == "string" && json.LabelBorderWidth.length > 0)
        reportView.LabelBorderWidth = json.LabelBorderWidth
    if (json.LabelFontStyle != void 0 && typeof (json.LabelFontStyle) == "string" && json.LabelFontStyle.length > 0)
        reportView.LabelFontStyle = json.LabelFontStyle;
    if (json.LabelUnderline != void 0 && typeof (json.LabelUnderline) == "boolean")
        reportView.LabelUnderline = json.LabelUnderline;
    if (json.LabelStrikeout != void 0 && typeof (json.LabelStrikeout) == "boolean")
        reportView.LabelStrikeout = json.LabelStrikeout;
    if (json.ReportCaptionAlign != void 0 && typeof (json.ReportCaptionAlign) == "string" && json.ReportCaptionAlign.length > 0)
        reportView.ReportCaptionAlign = json.ReportCaptionAlign;
    if (json.MarginBottom != void 0 && typeof (json.MarginBottom) == "string" && json.MarginBottom.length > 0)
        reportView.MarginBottom = json.MarginBottom;
    if (json.MarginLeft != void 0 && typeof (json.MarginLeft) == "string" && json.MarginLeft.length > 0)
        reportView.MarginLeft = json.MarginLeft;
    if (json.MarginRight != void 0 && typeof (json.MarginRight) == "string" && json.MarginRight.length > 0)
        reportView.MarginRight = json.MarginRight;
    if (json.MarginTop != void 0 && typeof (json.MarginTop) == "string" && json.MarginTop.length > 0)
        reportView.MarginTop = json.MarginTop;
    if (json.Items != void 0) {
        for (var index = 0; index <= json.Items.length - 1; index++) {
            var _reportItem = ReportItem.ParseJSON(json.Items[index]);
            if (_reportItem != void 0) reportView.Items.push(_reportItem);
        };
    };
    return reportView;
}

function ReportItem()
{
    this.reportItem = {
        TabularColumnWidth: '',
        Caption: '',
        CaptionHeight: '',
        CaptionWidth: '',
        CaptionX: '',
        CaptionY: '',
        CaptionFontName: 'Tahoma',
        CaptionFontSize: '12px',
        CaptionForeColor: 'black',
        CaptionBackColor: 'lightyellow',
        CaptionBorderColor: 'lightgrey',
        CaptionBorderStyle: 'Solid',
        CaptionBorderWidth: '1',
        CaptionFontStyle: 'Regular',
        CaptionUnderline: false, 
        CaptionStrikeout: false,
        CaptionTextAlign: 'Left',
        ReportItemType: enumItemType.DataField,
        ImagePath: '',
        ImageWidth: '',
        ImageHeight: '',
        ImageData: '',
        FieldLayout: enumFieldLayout.Block,
        SQLDatabase: '',
        SQLTable: '',
        SQLField: '',
        SQLDataType: '',
        ItemID: '',
        ItemOrder: '0',
        DataHeight: '',
        DataWidth: '',
        DataX: '',
        DataY: '',
        Height: '',
        Width: '',
        X: '',
        Y: '',
        FontName: 'Tahoma',
        FontSize: '12px',
        ForeColor: 'black',
        BackColor: 'white',
        BorderColor: 'lightgrey',
        BorderStyle: 'Solid',
        BorderWidth: '1',
        FontStyle: 'Regular',
        Underline: false,
        Strikeout: false,
        TextAlign: 'Left',
        Section: 'Details',
        ToJSON: function () {
            var __reportItem = {
                TabularColumnWidth: this.TabularColumnWidth || "",
                Caption: this.Caption || "",
                CaptionHeight: this.CaptionHeight || "",
                CaptionWidth: this.CaptionWidth || "",
                CaptionX: this.CaptionX || "",
                CaptionY: this.CaptionY || "",
                CaptionFontName: this.CaptionFontName || "",
                CaptionFontSize: this.CaptionFontSize || "",
                CaptionForeColor: this.CaptionForeColor || "black",
                CaptionBackColor: this.CaptionBackColor || "lightyellow",
                CaptionBorderColor: this.CaptionBorderColor || 'lightgrey',
                CaptionBorderStyle: this.CaptionBorderStyle || 'Solid',
                CaptionBorderWidth: this.CaptionBorderWidth || '1',
                CaptionFontStyle: this.CaptionFontStyle || 'Regular',
                CaptionUnderline: this.CaptionUnderline || false,
                CaptionStrikeout: this.CaptionStrikeout || false,
                CaptionTextAlign: this.CaptionTextAlign || 'Left',
                SQLDatabase: this.SQLDatabase || "",
                SQLTable: this.SQLTable || "",
                SQLField: this.SQLField || "",
                SQLDataType: this.SQLDataType || "",
                ItemID: this.ItemID || "",
                ItemOrder: this.ItemOrder || "0",
                DataHeight: this.DataHeight || "",
                DataWidth: this.DataWidth || "",
                DataX: this.DataX || "",
                DataY: this.DataY || "",
                Height: this.Height || "",
                Width: this.Width || "",
                X: this.X || "",
                Y: this.Y || "",
                FontName: this.FontName || "",
                FontSize: this.FontSize || "",
                ForeColor: this.ForeColor || "black",
                BackColor: this.BackColor || "white",
                BorderColor: this.BorderColor || 'lightgrey',
                BorderStyle: this.BorderStyle || 'Solid',
                BorderWidth: this.BorderWidth || '1',
                FontStyle: this.FontStyle || 'Regular',
                Underline: this.Underline || false,
                Strikeout: this.Strikeout || false,
                TextAlign: this.TextAlign || 'Left',
                Section: this.Section || "Details",
                ImagePath: this.ImagePath || "",
                ImageWidth: this.ImageWidth || "",
                ImageHeight: this.ImageHeight || "",
                ImageData: this.ImageData || ""
            };
            if (this.FieldLayout == enumFieldLayout.Block)
                __reportItem.FieldLayout = "Block";
            else if (this.FieldLayout==enumFieldLayout.Inline)
                __reportItem.FieldLayout = "Inline";
            else
                __reportItem.FieldLayout = "None"

            switch (this.ReportItemType) {
                case enumItemType.DataField:
                    __reportItem.ReportItemType = "DataField";
                    break;
                case enumItemType.FormulaField:
                    __reportItem.ReportItemType = "FormulaField";
                    break;
                case enumItemType.Section:
                    __reportItem.ReportItemType = "Section";
                    break;
                case enumItemType.Label:
                    __reportItem.ReportItemType = "Label";
                    break;
                case enumItemType.PageNumber:
                    __reportItem.ReportItemType = "PageNumber";
                    break;
                case enumItemType.PageTotal:
                    __reportItem.ReportItemType = "PageTotal";
                    break;
                case enumItemType.PageNofM:
                    __reportItem.ReportItemType = "PageNofM";
                    break;
                case enumItemType.ReportTitle:
                    __reportItem.ReportItemType = "ReportTitle";
                    break;
                case enumItemType.ReportUser:
                    __reportItem.ReportItemType = "ReportUser";
                    break;
                case enumItemType.ReportUserFL:
                    __reportItem.ReportItemType = "ReportUserFL";
                    break;
                case enumItemType.ReportUserFML:
                    __reportItem.ReportItemType = "ReportUserFML";
                    break;
                case enumItemType.ReportUserLF:
                    __reportItem.ReportItemType = "ReportUserLF";
                    break;
                case enumItemType.ReportUserLFM:
                    __reportItem.ReportItemType = "ReportUserLFM";
                    break;
                case enumItemType.ReportUserF:
                    __reportItem.ReportItemType = "ReportUserF";
                    break;
                case enumItemType.ReportUserM:
                    __reportItem.ReportItemType = "ReportUserM";
                    break;
                case enumItemType.ReportUserL:
                    __reportItem.ReportItemType = "ReportUserL";
                    break;
                case enumItemType.PrintDate:
                    __reportItem.ReportItemType = "PrintDate";
                    break;
                case enumItemType.PrintTime:
                    __reportItem.ReportItemType = "PrintTime";
                    break;
                case enumItemType.PrintDateTime:
                    __reportItem.ReportItemType = "PrintDateTime";
                    break;
                case enumItemType.Query:
                    __reportItem.ReportItemType = "SqlQuery";
                    break;
                case enumItemType.ConfidentialityStatement:
                    __reportItem.ReportItemType = "ConfidentialityStatement";
                    break;
                case enumItemType.ReportComments:
                    __reportItem.ReportItemType = "ReportComments";
                    break;
                case enumItemType.Image:
                    __reportItem.ReportItemType = "Image";
                    break;
                case enumItemType.Group:
                   __reportItem.ReportItemType = "Group";
                    break;
                default:
                    __reportItem.ReportItemType = "DataField";
            }
            return __reportItem;
        }
    };
    return this.reportItem;
}

ReportItem.ParseJSON = function (json, reportItem) {
        
    if (json.Caption != void 0) {
        reportItem = reportItem || new ReportItem();
        if (json.TabularColumnWidth != void 0 && typeof (json.TabularColumnWidth) == "string" && json.TabularColumnWidth.length > 0)
            reportItem.TabularColumnWidth = json.TabularColumnWidth;
        if (json.Caption != void 0 && typeof (json.Caption) == "string" && json.Caption.length > 0)
            reportItem.Caption = json.Caption;
        if (json.CaptionHeight != void 0 && typeof (json.CaptionHeight) == "string" && json.CaptionHeight.length > 0)
            reportItem.CaptionHeight = json.CaptionHeight;
        if (json.CaptionWidth != void 0 && typeof (json.CaptionWidth) == "string" && json.CaptionWidth.length > 0)
            reportItem.CaptionWidth = json.CaptionWidth;
        if (json.CaptionX != void 0 && typeof (json.CaptionX) == "string" && json.CaptionX.length > 0)
            reportItem.CaptionX = json.CaptionX;
        if (json.CaptionY != void 0 && typeof (json.CaptionY) == "string" && json.CaptionY.length > 0)
            reportItem.CaptionY = json.CaptionY;
        if (json.CaptionFontName != void 0 && typeof (json.CaptionFontName) == "string" && json.CaptionFontName.length > 0)
            reportItem.CaptionFontName = json.CaptionFontName;
        if (json.CaptionFontSize != void 0 && typeof (json.CaptionFontSize) == "string" && json.CaptionFontSize.length > 0)
            reportItem.CaptionFontSize = json.CaptionFontSize;
        if (json.CaptionForeColor != void 0 && typeof (json.CaptionForeColor) == "string" && json.CaptionForeColor.length > 0)
            reportItem.CaptionForeColor = json.CaptionForeColor;
        if (json.CaptionBackColor != void 0 && typeof (json.CaptionBackColor) == "string" && json.CaptionBackColor.length > 0)
            reportItem.CaptionBackColor = json.CaptionBackColor;
        if (json.CaptionBorderColor != void 0 && typeof (json.CaptionBorderColor) == "string" && json.CaptionBorderColor.length > 0)
            reportItem.CaptionBorderColor = json.CaptionBorderColor;
        if (json.CaptionBorderStyle != void 0 && typeof (json.CaptionBorderStyle) == "string" && json.CaptionBorderStyle.length > 0)
            reportItem.CaptionBorderStyle = json.CaptionBorderStyle;
        if (json.CaptionBorderWidth != void 0 && typeof (json.CaptionBorderWidth) == "string" && json.CaptionBorderWidth.length > 0)
            reportItem.CaptionBorderWidth = json.CaptionBorderWidth;
        if (json.CaptionFontStyle != void 0 && typeof (json.CaptionFontStyle) == "string" && json.CaptionFontStyle.length > 0)
            reportItem.CaptionFontStyle = json.CaptionFontStyle;
        if (json.CaptionUnderline != void 0 && typeof (json.CaptionUnderline) == "boolean")
            reportItem.CaptionUnderline = json.CaptionUnderline;
        if (json.CaptionStrikeout != void 0 && typeof (json.CaptionStrikeout) == "boolean")
            reportItem.CaptionStrikeout = json.CaptionStrikeout;
        if (json.CaptionTextAlign != void 0 && typeof (json.CaptionTextAlign == "string") && json.CaptionTextAlign.length > 0)
            reportItem.CaptionTextAlign = json.CaptionTextAlign;
        if (json.FieldLayout != void 0 && typeof (json.FieldLayout == "string") && json.FieldLayout.length > 0) {
            switch (json.FieldLayout) {
                case "Inline":
                    reportItem.FieldLayout = enumFieldLayout.Inline;
                    break;
                case "Block":
                    reportItem.FieldLayout = enumFieldLayout.Block;
                    break;
                case "None":
                    reportItem.FieldLayout = enumFieldLayout.None;
                    break;

            }
        }
        else
            reportItem.FieldLayout = enumFieldLayout.Block;

        if (json.ReportItemType != void 0 && typeof (json.ReportItemType) == "string" && json.ReportItemType.length > 0) {
            switch (json.ReportItemType) {
                case "DataField":
                    reportItem.ReportItemType = enumItemType.DataField;
                    break;
                case "FormulaField":
                    reportItem.ReportItemType = enumItemType.FormulaField;
                    break;
                case "Section":
                    reportItem.ReportItemType = enumItemType.Section;
                    break;
                case "Label":
                    reportItem.ReportItemType = enumItemType.Label;
                    break;
                case "PageNumber":
                    reportItem.ReportItemType = enumItemType.PageNumber;
                    break;
                case "PageTotal":
                    reportItem.ReportItemType = enumItemType.PageTotal;
                    break;
                case "PageNofM":
                    reportItem.ReportItemType = enumItemType.PageNofM;
                    break;
                case "ReportTitle":
                    reportItem.ReportItemType = enumItemType.ReportTitle;
                    break;
                case "ReportUser":
                    reportItem.ReportItemType = enumItemType.ReportUser;
                    break;
                case "ReportUserFML":
                    reportItem.ReportItemType = enumItemType.ReportUserFML;
                    break;
                case "ReportUserLF":
                    reportItem.ReportItemType = enumItemType.ReportUserLF;
                    break;
                case "ReportUserLFM":
                    reportItem.ReportItemType = enumItemType.ReportUserLFM;
                    break;
                case "ReportUserF":
                    reportItem.ReportItemType = enumItemType.ReportUserF;
                    break;
                case "ReportUserM":
                    reportItem.ReportItemType = enumItemType.ReportUserM;
                    break;
                case "ReportUserL":
                    reportItem.ReportItemType = enumItemType.ReportUserL;
                    break;
                case "PrintDate":
                    reportItem.ReportItemType = enumItemType.PrintDate;
                    break;
                case "PrintTime":
                    reportItem.ReportItemType = enumItemType.PrintTime;
                    break;
                case "PrintDateTime":
                    reportItem.ReportItemType = enumItemType.PrintDateTime;
                    break;
                case "SqlQuery":
                    reportItem.ReportItemType = enumItemType.Query;
                    break;
                case "ConfidentialityStatement":
                    reportItem.ReportItemType = enumItemType.ConfidentialityStatement;
                    break;
                case "ReportComments":
                    reportItem.ReportItemType = enumItemType.ReportComments;
                    break;
                case "Image":
                   reportItem.ReportItemType = enumItemType.Image;
                    break;
                case "Group":
                    reportItem.ReportItemType = enumItemType.Group;
                    break;
                default:
                    reportItem.ReportItemType = enumItemType.DataField;

            }
        }
        else
            reportItem.ReportItemType = enumItemType.DataField;

        if (json.ImagePath != void 0 && typeof (json.ImagePath) == "string" && json.ImagePath.length > 0)
            reportItem.ImagePath = json.ImagePath;
        if (json.ImageWidth != void 0 && typeof (json.ImageWidth) == "string" && json.ImageWidth.length > 0)
            reportItem.ImageWidth = json.ImageWidth;
        if (json.ImageHeight != void 0 && typeof (json.ImageHeight) == "string" && json.ImageHeight.length > 0)
            reportItem.ImageHeight = json.ImageHeight;
        if (json.ImageData != void 0 && typeof (json.ImageData) == "string" && json.ImageData.length > 0)
            reportItem.ImageData = json.ImageData;
        if (json.SQLDatabase != void 0 && typeof (json.SQLDatabase) == "string" && json.SQLDatabase.length > 0)
            reportItem.SQLDatabase = json.SQLDatabase;
        if (json.SQLTable != void 0 && typeof (json.SQLTable) == "string" && json.SQLTable.length > 0)
            reportItem.SQLTable = json.SQLTable;
        if (json.SQLField != void 0 && typeof (json.SQLField) == "string" && json.SQLField.length > 0)
            reportItem.SQLField = json.SQLField;
        if (json.SQLDataType != void 0 && typeof (json.SQLDataType) == "string" && json.SQLDataType.length > 0)
            reportItem.SQLDataType = json.SQLDataType;
        if (json.ItemID != void 0 && typeof (json.ItemID) == "string" && json.ItemID.length > 0)
            reportItem.ItemID = json.ItemID;
        if (json.ItemOrder != void 0 && typeof (json.ItemOrder) == "string" && json.ItemOrder.length > 0)
            reportItem.ItemOrder = json.ItemOrder;
        if (json.DataHeight != void 0 && typeof (json.DataHeight) == "string" && json.DataHeight.length > 0)
            reportItem.DataHeight = json.DataHeight;
        if (json.DataWidth != void 0 && typeof (json.DataWidth) == "string" && json.DataWidth.length > 0)
            reportItem.DataWidth = json.DataWidth;
        if (json.DataX != void 0 && typeof (json.DataX) == "string" && json.DataX.length > 0)
            reportItem.DataX = json.DataX;
        if (json.DataY != void 0 && typeof (json.DataY) == "string" && json.DataY.length > 0)
            reportItem.DataY = json.DataY;
        if (json.Height != void 0 && typeof (json.Height) == "string" && json.Height.length > 0)
            reportItem.Height = json.Height;
        if (json.Width != void 0 && typeof (json.Width) == "string" && json.Width.length > 0)
            reportItem.Width = json.Width;
        if (json.X != void 0 && typeof (json.X) == "string" && json.X.length > 0)
            reportItem.X = json.X;
        if (json.Y != void 0 && typeof (json.Y) == "string" && json.Y.length > 0)
            reportItem.Y = json.Y;
        if (json.FontName != void 0 && typeof (json.FontName) == "string" && json.FontName.length > 0)
            reportItem.FontName = json.FontName;
        if (json.FontSize != void 0 && typeof (json.FontSize) == "string" && json.FontSize.length > 0)
            reportItem.FontSize = json.FontSize;
        if (json.ForeColor != void 0 && typeof (json.ForeColor) == "string" && json.ForeColor.length > 0)
            reportItem.ForeColor = json.ForeColor;
        if (json.BackColor != void 0 && typeof (json.BackColor) == "string" && json.BackColor.length > 0)
            reportItem.BackColor = json.BackColor;
        if (json.BorderColor != void 0 && typeof (json.BorderColor) == "string" && json.BorderColor.length > 0)
            reportItem.BorderColor = json.BorderColor;
        if (json.BorderStyle != void 0 && typeof (json.BorderStyle) == "string" && json.BorderStyle.length > 0)
            reportItem.BorderStyle = json.BorderStyle;
        if (json.BorderWidth != void 0 && typeof (json.BorderWidth) == "string" && json.BorderWidth.length > 0)
            reportItem.BorderWidth = json.BorderWidth;
        if (json.FontStyle != void 0 && typeof (json.FontStyle) == "string" && json.FontStyle.length > 0)
            reportItem.FontStyle = json.FontStyle;
        if (json.Underline != void 0 && typeof (json.Underline) == "boolean")
            reportItem.Underline = json.Underline;
        if (json.Strikeout != void 0 && typeof (json.Strikeout) == "boolean")
            reportItem.Strikeout = json.Strikeout;
        if (json.TextAlign != void 0 && typeof (json.TextAlign == "string") && json.TextAlign.length > 0)
            reportItem.TextAlign = json.TextAlign;

        if (json.Section != void 0 && typeof (json.Section) == "string" && json.Section.length > 0)
            reportItem.Section = json.Section;

        return reportItem;
    }
    return null;
}

// ********************************** FontData ********************************
function FontData() {
    this.fontData = {
        Items: [],
        _init: function () {
            this.Items.IndexOf = function (fontname) {
                for (var index = 0 ; index < this.length; index++) {
                    if (this[index].Name == fontname)
                        return index;
                }
                return -1;
            };

            this.Items.GetItem = function (fontname) {
                var idx = this.IndexOf(fontname);
                if (idx != -1)
                    return this[idx];
                return null;
            }
        }
    }
    this.fontData._init();
    return this.fontData;
}

FontData.ParseJSON = function (json, fontData) {
    fontData = fontData || new FontData();

    if (json.Items != void 0) {
        for (var index = 0; index < json.Items.length; index++) {
            var _fontItem = FontItem.ParseJSON(json.Items[index]);
            if (_fontItem != void 0) fontData.Items.push(_fontItem);
        };
    };
}

function FontItem() {
    this.fontItem = {
        Name: '',
        BoldAvailable: true,
        ItalicAvailable: true,
        RegularAvailable: true,
        StrikeoutAvailable: true,
        UnderlineAvailable: true
    };
    return this.fontItem;
}

FontItem.ParseJSON = function (json, fontItem) {
    if (json.name != void 0) {
        fontItem = fontItem || new FontItem();
        if (json.name != void 0 && typeof (json.name) == "string" && json.name.length > 0)
            fontItem.Name = json.name;
        if (json.boldAvailable != void 0 && typeof (json.boldAvailable) == "boolean")
            fontItem.BoldAvailable = json.boldAvailable;
        if (json.italicAvailable != void 0 && typeof (json.italicAvailable) == "boolean")
            fontItem.ItalicAvailable = json.italicAvailable;
        if (json.regularAvailable != void 0 && typeof (json.regularAvailable) == "boolean")
            fontItem.RegularAvailable = json.regularAvailable;
        if (json.strikeoutAvailable != void 0 && typeof (json.strikeoutAvailable) == "boolean")
            fontItem.StrikeoutAvailable = json.strikeoutAvailable;
        if (json.underlineAvailable != void 0 && typeof (json.underlineAvailable) == "boolean")
            fontItem.UnderlineAvailable = json.underlineAvailable;

        return fontItem;
    }
    return null;
};
// ********************************** FontSettings *******************************
function FontSettings() {
    this.fontSettings = {
        FontName:'Tahoma',
        FontStyle:'Regular',
        FontSize:'12px',
        Strikeout: false,
        Underline: false,
        ColorName:'black',
        ColorHex:'#000000'
    }
    return this.fontSettings;
}

// ********************************** DisplaySettings *******************************
function DisplaySettings() {
    this.displaySettings = {
        BackColorName: 'white',
        BackColorHex: '#FFFFFF',
        BorderColorName: 'lightgrey',
        BorderColorHex: '#D3D3D3',
        BorderStyle: 'Solid',
        BorderWidth: '1'
    }
    return this.displaySettings;
}

// ******************************** HeaderFooterSettings ************************
function HeaderFooterSettings() {
    this.headerFooterSettings = {
        Title: '',
        Height: "1",
        BackColorName: 'white',
        BackColorHex: '#FFFFFF',
        BorderColorName: 'lightgrey',
        BorderColorHex: '#D3D3D3',
        BorderStyle: 'Solid',
        BorderWidth: '1',
        FieldSettings:  new HeaderFooterFieldSettings()
    }
    return this.headerFooterSettings;
}

// ******************************** HeaderFooterFieldSettings ************************
function HeaderFooterFieldSettings() {
    this.headerFooterFieldSettings = {
        UseBackColor:  true,
        BackColorName: 'white',
        BackColorHex: '#FFFFFF',
        UseTextColor: false,
        TextColorName: 'black',
        TextColorHex: '#000000',
        RemoveBorders: true
    }
    return this.headerFooterFieldSettings;
}

// ****************************** ImageFieldSettings ****************************
function ImageFieldSettings() {
    this.imageFieldSettings = {
        ImageWidth: '64',
        ImageHeight: '64',
        AspectRatio: '',
        SizeOption: "Square",
        ImagePath: '',
        ImageData: '',
        ImageType: '',
        BorderColorName: 'lightgrey',
        BorderColorHex: '#D3D3D3',
        BorderStyle: 'None',
        BorderWidth: '1',
    }
    return this.imageFieldSettings;
}

// ***************************************************************************

var availableFonts = [];
var fsCaption = new FontSettings();
var fsDetail = new FontSettings();
var fsLabel = new FontSettings();
var fsHeaderFieldSettings = new FontSettings();
var fsGroupFieldSettings = new FontSettings();
var fsFooterFieldSettings = new FontSettings();
var dsCaption = new DisplaySettings();
var dsDetail = new DisplaySettings();
var dsLabel = new DisplaySettings();
var dsHeaderFieldSettings = new DisplaySettings();
var dsFooterFieldSettings = new DisplaySettings();
var dsGroupFieldSettings = new DisplaySettings();
var dsHeader = new HeaderFooterSettings();
var dsFooter = new HeaderFooterSettings();
var dsImageFieldSettings = new ImageFieldSettings();

var pageX,curCol,nxtCol,curColWidth,nxtColWidth,styles;

const wordToHex = {
    aliceblue: "#F0F8FF",
    antiquewhite: "#FAEBD7",
    aqua: "#00FFFF",
    aquamarine: "#7FFFD4",
    azure: "#F0FFFF",
    beige: "#F5F5DC",
    bisque: "#FFE4C4",
    black: "#000000",
    blanchedalmond: "#FFEBCD",
    blue: "#0000FF",
    blueviolet: "#8A2BE2",
    brown: "#A52A2A",
    burlywood: "#DEB887",
    cadetblue: "#5F9EA0",
    chartreuse: "#7FFF00",
    chocolate: "#D2691E",
    coral: "#FF7F50",
    cornflowerblue: "#6495ED",
    cornsilk: "#FFF8DC",
    crimson: "#DC143C",
    cyan: "#00FFFF",
    darkblue: "#00008B",
    darkcyan: "#008B8B",
    darkgoldenrod: "#B8860B",
    darkgray: "#A9A9A9",
    //darkgrey: "#A9A9A9",
    darkgreen: "#006400",
    darkkhaki: "#BDB76B",
    darkmagenta: "#8B008B",
    darkolivegreen: "#556B2F",
    darkorange: "#FF8C00",
    darkorchid: "#9932CC",
    darkred: "#8B0000",
    darksalmon: "#E9967A",
    darkseagreen: "#8FBC8F",
    darkslateblue: "#483D8B",
    darkslategray: "#2F4F4F",
    //darkslategrey: "#2F4F4F",
    darkturquoise: "#00CED1",
    darkviolet: "#9400D3",
    deeppink: "#FF1493",
    deepskyblue: "#00BFFF",
    dimgray: "#696969",
    //dimgrey: "#696969",
    dodgerblue: "#1E90FF",
    firebrick: "#B22222",
    floralwhite: "#FFFAF0",
    forestgreen: "#228B22",
    fuchsia: "#FF00FF",
    gainsboro: "#DCDCDC",
    ghostwhite: "#F8F8FF",
    gold: "#FFD700",
    goldenrod: "#DAA520",
    gray: "#808080",
    //grey: "#808080",
    green: "#008000",
    greenyellow: "#ADFF2F",
    honeydew: "#F0FFF0",
    hotpink: "#FF69B4",
    indianred: "#CD5C5C",
    indigo: "#4B0082",
    ivory: "#FFFFF0",
    khaki: "#F0E68C",
    lavender: "#E6E6FA",
    lavenderblush: "#FFF0F5",
    lawngreen: "#7CFC00",
    lemonchiffon: "#FFFACD",
    lightblue: "#ADD8E6",
    lightcoral: "#F08080",
    lightcyan: "#E0FFFF",
    lightgoldenrodyellow: "#FAFAD2",
    //lightgray: "#D3D3D3",
    lightgrey: "#D3D3D3",
    lightgreen: "#90EE90",
    lightpink: "#FFB6C1",
    lightsalmon: "#FFA07A",
    lightseagreen: "#20B2AA",
    lightskyblue: "#87CEFA",
    lightslategray: "#778899",
    //lightslategrey: "#778899",
    lightsteelblue: "#B0C4DE",
    lightyellow: "#FFFFE0",
    lime: "#00FF00",
    limegreen: "#32CD32",
    linen: "#FAF0E6",
    magenta: "#FF00FF",
    maroon: "#800000",
    mediumaquamarine: "#66CDAA",
    mediumblue: "#0000CD",
    mediumorchid: "#BA55D3",
    mediumpurple: "#9370DB",
    mediumseagreen: "#3CB371",
    mediumslateblue: "#7B68EE",
    mediumspringgreen: "#00FA9A",
    mediumturquoise: "#48D1CC",
    mediumvioletred: "#C71585",
    midnightblue: "#191970",
    mintcream: "#F5FFFA",
    mistyrose: "#FFE4E1",
    moccasin: "#FFE4B5",
    navajowhite: "#FFDEAD",
    navy: "#000080",
    oldlace: "#FDF5E6",
    olive: "#808000",
    olivedrab: "#6B8E23",
    orange: "#FFA500",
    orangered: "#FF4500",
    orchid: "#DA70D6",
    palegoldenrod: "#EEE8AA",
    palegreen: "#98FB98",
    paleturquoise: "#AFEEEE",
    palevioletred: "#DB7093",
    papayawhip: "#FFEFD5",
    peachpuff: "#FFDAB9",
    peru: "#CD853F",
    pink: "#FFC0CB",
    plum: "#DDA0DD",
    powderblue: "#B0E0E6",
    purple: "#800080",
    rebeccapurple: "#663399",
    red: "#FF0000",
    rosybrown: "#BC8F8F",
    royalblue: "#4169E1",
    saddlebrown: "#8B4513",
    salmon: "#FA8072",
    sandybrown: "#F4A460",
    seagreen: "#2E8B57",
    seashell: "#FFF5EE",
    sienna: "#A0522D",
    silver: "#C0C0C0",
    skyblue: "#87CEEB",
    slateblue: "#6A5ACD",
    slategray: "#708090",
    //slategrey: "#708090",
    snow: "#FFFAFA",
    springgreen: "#00FF7F",
    steelblue: "#4682B4",
    tan: "#D2B48C",
    teal: "#008080",
    thistle: "#D8BFD8",
    tomato: "#FF6347",
    turquoise: "#40E0D0",
    violet: "#EE82EE",
    wheat: "#F5DEB3",
    white: "#FFFFFF",
    whitesmoke: "#F5F5F5",
    yellow: "#FFFF00",
    yellowgreen: "#9ACD32",
};

const hexToWord =  Object.fromEntries(
    Object.entries(wordToHex)
    .map(([k, v]) => [v, k])
)

const toHex = (color) => wordToHex[color.toLowerCase()];
const fromHex = (hex) => hexToWord[hex.toUpperCase()];

//******************************************************* Functions ************************************************
function loadReportDisplay() {
    if (isLoaded) {
        var divDrop = document.getElementById("divDrop");
        var divHeader = document.getElementById("divHeader");
        var divFooter = document.getElementById("divFooter");
        var divGroup = document.getElementById("divGroup");

        // create default settings objects
        var fs = new FontSettings();
        var ds = new DisplaySettings();
        var ifs = new ImageFieldSettings();

        var reportTitle = document.getElementById("hdnReportTitle").value;

        if (divDrop != void 0) {
            var itm;
            var id;
            var caption;

            var left;
            var top;
            var height;
            var width;

            var captionleft;
            var captiontop;
            var captionheight;
            var captionwidth;

            var dataleft;
            var datatop;
            var dataheight;
            var datawidth;

            var fieldLayout;
            var fieldFormat;
            var div;
            var parts;
            var divCaption;
            var divField;
            var fieldno;
            var section;
            var divRect;
            var dropRect = divDrop.getBoundingClientRect();
            var isRectBlank;
            var hasHeightWidth;

            //dsHeader
            dsHeader.Title = reportTitle;
            dsHeader.Height = reportView.HeaderHeight;
            dsHeader.BackColorName = reportView.HeaderBackColor;
            if (dsHeader.BackColorName.startsWith("#"))
                dsHeader.BackColorHex = dsHeader.BackColorName;
            else
                dsHeader.BackColorHex = toHex(dsHeader.BackColorName);

            dsHeader.FieldSettings.BackColorName = dsHeader.BackColorName;
            dsHeader.FieldSettings.BackColorHex = dsHeader.BackColorHex;
            dsHeaderFieldSettings.BackColorName = dsHeader.BackColorName;
            dsHeaderFieldSettings.BackColorHex = dsHeader.BackColorHex;

            dsHeader.BorderColorName = reportView.HeaderBorderColor;
            if (dsHeader.BorderColorName.startsWith("#"))
                dsHeader.BorderColorHex = dsHeader.BorderColorName;
            else
                dsHeader.BorderColorHex = toHex(dsHeader.BorderColorName);

            dsHeader.BorderStyle = reportView.HeaderBorderStyle;
            dsHeaderFieldSettings.BorderStyle = "None"
            dsHeader.BorderWidth = reportView.HeaderBorderWidth;
            applyHeaderFooterSettings("header");

            //dsFooter
            dsFooter.Title = reportTitle;
            dsFooter.Height = reportView.FooterHeight;
            dsFooter.BackColorName = reportView.FooterBackColor;
            if (dsFooter.BackColorName.startsWith("#"))
                dsFooter.BackColorHex = dsFooter.BackColorName;
            else
                dsFooter.BackColorHex = toHex(dsFooter.BackColorName);

            dsFooter.FieldSettings.BackColorName = dsFooter.BackColorName;
            dsFooter.FieldSettings.BackColorHex = dsFooter.BackColorHex;
            dsFooterFieldSettings.BackColorName = dsFooter.BackColorName;
            dsFooterFieldSettings.BackColorHex = dsFooter.BackColorHex;

            dsFooter.BorderColorName = reportView.FooterBorderColor;
            if (dsFooter.BorderColorName.startsWith("#"))
                dsFooter.BorderColorHex = dsFooter.BorderColorName;
            else
                dsFooter.BorderColorHex = toHex(dsFooter.BorderColorName);

            dsFooter.BorderStyle = reportView.FooterBorderStyle;
            dsFooterFieldSettings.BorderStyle = "None";
            dsFooter.BorderWidth = reportView.FooterBorderWidth;
            applyHeaderFooterSettings("footer");

            // fsCaption
            fsCaption.FontName = reportView.LabelFontName;
            fsCaption.FontStyle = reportView.LabelFontStyle;
            fsCaption.FontSize = reportView.LabelFontSize;
            fsCaption.Underline = reportView.LabelUnderline;
            fsCaption.Strikeout = reportView.LabelStrikeout;
            fsCaption.ColorName = reportView.LabelForeColor;
            if (fsCaption.ColorName.startsWith("#"))
                fsCaption.ColorHex = fsCaption.ColorName;
            else
                fsCaption.ColorHex = toHex(fsCaption.ColorName);

            // fsDetail
            fsDetail.FontName = reportView.DataFontName;
            fsDetail.FontStyle = reportView.DataFontStyle;
            fsDetail.FontSize = reportView.DataFontSize;
            fsDetail.Underline = reportView.DataUnderline;
            fsDetail.Strikeout = reportView.DataStrikeout;
            fsDetail.ColorName = reportView.DataForeColor;
            if (fsDetail.ColorName.startsWith("#"))
                fsDetail.ColorHex = fsDetail.ColorName;
            else
                fsDetail.ColorHex = toHex(fsDetail.ColorName);

            // copy fsDetail
            fs = JSON.parse(JSON.stringify(fsDetail));

            // dsCaption
            dsCaption.BackColorName = reportView.LabelBackColor;
            if (dsCaption.BackColorName.startsWith("#"))
                dsCaption.BackColorHex = dsCaption.BackColorName;
            else
                dsCaption.BackColorHex = toHex(dsCaption.BackColorName);

            dsCaption.BorderColorName = reportView.LabelBorderColor;
            if (dsCaption.BorderColorName.startsWith("#"))
                dsCaption.BorderColorHex = dsCaption.BorderColorName;
            else
                dsCaption.BorderColorHex = toHex(dsCaption.BorderColorName);

            dsCaption.BorderStyle = reportView.LabelBorderStyle;
            dsCaption.BorderWidth = reportView.LabelBorderWidth;

            // dsDetail
            dsDetail.BackColorName = reportView.DataBackColor;
            if (dsDetail.BackColorName.startsWith("#"))
                dsDetail.BackColorHex = dsDetail.BackColorName;
            else
                dsDetail.BackColorHex = toHex(dsDetail.BackColorName);

            dsDetail.BorderColorName = reportView.DataBorderColor;
            if (dsDetail.BorderColorName.startsWith("#"))
                dsDetail.BorderColorHex = dsDetail.BorderColorName;
            else
                dsDetail.BorderColorHex = toHex(dsDetail.BorderColorName);

            dsDetail.BorderStyle = reportView.DataBorderStyle;
            dsDetail.BorderWidth = reportView.DataBorderWidth;
            // copy dsDetail
            ds = JSON.parse(JSON.stringify(dsDetail));

            for (var i = 0; i < reportView.Items.length; i++) {
                itm = reportView.Items[i];

                id = itm.ItemID;
                caption = itm.Caption;
                left = (itm.X != "") ? parseInt(itm.X,10):"";
                top = (itm.Y != "") ? parseInt(itm.Y,10):"";
                width = (itm.Width != "") ? parseInt(itm.Width,10):"";
                height = (itm.Height != "") ? parseInt(itm.Height,10):"";

                captionleft = itm.CaptionX;
                captiontop = itm.CaptionY;
                captionwidth = itm.CaptionWidth;
                captionheight = itm.CaptionHeight;

                dataleft = itm.DataX;
                datatop = itm.DataY;
                datawidth = itm.DataWidth;
                dataheight = itm.DataHeight;


                fieldLayout = itm.FieldLayout;
                section = itm.Section;
                div = null;
                isRectBlank = (left != "" && top != "") ? false:true;
                hasHeightWidth = (height != "" && width != "");

                if (itm.ReportItemType == enumItemType.Label) {
                    fsLabel.FontName = itm.CaptionFontName;
                    fsLabel.FontStyle = itm.CaptionFontStyle;
                    fsLabel.FontSize = itm.CaptionFontSize;
                    fsLabel.Underline = itm.CaptionUnderline;
                    fsLabel.Strikeout = itm.CaptionStrikeout;
                    fsLabel.ColorName = itm.CaptionForeColor;
                    if (itm.CaptionForeColor.startsWith("#"))
                        fsLabel.ColorHex = itm.CaptionForeColor;
                    else
                        fsLabel.ColorHex = toHex(itm.CaptionForeColor);

                    dsLabel.BorderStyle = itm.CaptionBorderStyle;
                    dsLabel.BorderWidth = itm.CaptionBorderWidth;
                    dsLabel.BackColorName=itm.CaptionBackColor;
                    if (itm.CaptionBackColor.startsWith("#"))
                        dsLabel.BackColorHex = itm.CaptionBackColor;
                    else
                        dsLabel.BackColorHex = toHex(itm.CaptionBackColor);

                    dsLabel.BorderColorName = itm.CaptionBorderColor;
                    if (itm.CaptionBorderColor.startsWith("#"))
                        dsLabel.BorderColorHex = itm.CaptionBorderColor;
                    else
                        dsLabel.BackColorHex = toHex(itm.CaptionBorderColor);
                }
                else if (itm.ReportItemType == enumItemType.Group) {
                    fsGroupFieldSettings.fontname = itm.FontName
                    fsGroupFieldSettings.FontStyle = itm.FontStyle;
                    fsGroupFieldSettings.FontSize = itm.FontSize;
                    fsGroupFieldSettings.Underline = itm.Underline;
                    fsGroupFieldSettings.Strikeout = itm.Strikeout;
                    fsGroupFieldSettings.ColorName = itm.ForeColor;
                    if (itm.ForeColor.startsWith("#"))
                        fsGroupFieldSettings.ColorHex = itm.ForeColor;
                    else
                        fsGroupFieldSettings.ColorHex = toHex(itm.ForeColor);

                    dsGroupFieldSettings.BorderStyle = itm.BorderStyle;
                    dsGroupFieldSettings.BorderWidth = itm.BorderWidth;
                    dsGroupFieldSettings.BackColorName = itm.BackColor;
                    if (itm.CaptionBackColor.startsWith("#"))
                        dsGroupFieldSettings.BackColorHex = itm.BackColor;
                    else
                        dsGroupFieldSettings.BackColorHex = toHex(itm.BackColor);

                    dsGroupFieldSettings.BorderColorName = itm.BorderColor;
                    if (itm.CaptionBorderColor.startsWith("#"))
                        dsGroupFieldSettings.BorderColorHex = itm.BorderColor;
                    else
                        dsGroupFieldSettings.BackColorHex = toHex(itm.BorderColor);


                }
                else if (itm.ReportItemType != enumItemType.DataField && itm.ReportItemType != enumItemType.Group) {
                    fsDetail.FontName = itm.FontName;
                    fsDetail.FontStyle = itm.FontStyle;
                    fsDetail.FontSize = itm.FontSize;
                    fsDetail.Underline = itm.Underline;
                    fsDetail.Strikeout = itm.Strikeout;
                    fsDetail.ColorName = itm.ForeColor;
                    if (itm.ForeColor.startsWith("#"))
                        fsDetail.ColorHex = itm.ForeColor;
                    else
                        fsDetail.ColorHex = toHex(itm.ForeColor);


                    dsDetail.BorderStyle = itm.BorderStyle;
                    dsDetail.BorderWidth = itm.BorderWidth;
                    dsDetail.BackColorName=itm.BackColor;
                    if (itm.CaptionBackColor.startsWith("#"))
                        dsDetail.BackColorHex = itm.BackColor;
                    else
                        dsDetail.BackColorHex = toHex(itm.BackColor);

                    dsDetail.BorderColorName = itm.BorderColor;
                    if (itm.CaptionBorderColor.startsWith("#"))
                        dsDetail.BorderColorHex = itm.BorderColor;
                    else
                        dsDetail.BackColorHex = toHex(itm.BorderColor);

                }

                if (section == "Details") {
                    if (itm.ReportItemType == enumItemType.DataField) {
                        div = createDivFromItem(itm);
                        divDrop.appendChild(div);
                        fieldno = divDrop.children.length - 1;
                        div.setAttribute("data-fieldindex", fieldno);

                        divCaption = div.children[0];
                        divField = div.children[1];
                        divRect = div.getBoundingClientRect();
                        if (isRectBlank) {
                            left = parseInt(divRect.left - dropRect.left, 10);
                            top = parseInt(divRect.top - dropRect.top, 10);
                            width = parseInt(divRect.width, 10);
                            height = parseInt(divRect.height, 10);
                        }
                        fieldFormat = "Block";
                        if (reportView.ReportTemplate == enumReportTemplate.FreeForm) {
                            if (fieldLayout == enumFieldLayout.Inline)
                                fieldFormat = "Inline";
                            if (left != "" && top != "") {
                                div.style.position = "absolute";
                                div.style.left = left + "px";
                                div.style.top = top + "px";
                                if (fieldFormat == "Inline") {
                                    divCaption.style.margin = "0px 5px 0px 0px";
                                    divCaption.style.cssFloat = "left";
                                    divField.style.margin = "0px 0px 0px 0px";
                                }
                                else {
                                    divCaption.style.margin = "5px";
                                    divField.style.margin = "5px";
                                    divCaption.style.cssFloat = "";
                                }
                            }
                        }
                        div.setAttribute("data-fieldformat", fieldFormat);

                        var rctCaption = divCaption.getBoundingClientRect();
                        var rctField = divField.getBoundingClientRect();

                        isRectBlank = (captionleft != "" && captiontop != "") ? false : true;
                        if (isRectBlank) {
                            captionleft = left;
                            captiontop = top;
                            captionwidth = parseInt(rctCaption.width, 10);
                            captionheight = parseInt(rctCaption.height, 10);
                        }
                        //captionleft = itm.CaptionX;
                        //captiontop = itm.CaptionY;
                        //captionwidth = itm.CaptionWidth;
                        //captionheight = itm.CaptionHeight;

                        //dataleft = itm.DataX;
                        //datatop = itm.DataY;
                        //datawidth = itm.DataWidth;
                        //dataheight = itm.DataHeight;

                        divCaption.setAttribute("data-dropwidth", parseInt(captionwidth, 10));
                        divCaption.setAttribute("data-dropheight", parseInt(captionheight, 10));
                        divCaption.setAttribute("data-dropleft", parseInt(captionleft, 10));
                        divCaption.setAttribute("data-droptop", parseInt(captiontop, 10));

                        isRectBlank = (dataleft != "" && datatop != "") ? false : true;
                        if (isRectBlank) {
                            if (fieldFormat.toUpperCase() == "INLINE") {
                                dataleft = parseInt(left + captionwidth + 3);
                                datatop = top;
                            }
                            else if (fieldFormat.toUpperCase() == "BLOCK") {
                                dataleft = left;
                                datatop = parseInt(top + captionheight + 3);
                            }

                            datawidth = parseInt(rctField.width, 10);
                            dataheight = parseInt(rctField.height, 10);
                        }
                        divField.setAttribute("data-dropwidth", parseInt(datawidth, 10));
                        divField.setAttribute("data-dropheight", parseInt(dataheight, 10));
                        divField.setAttribute("data-dropleft", parseInt(dataleft, 10));
                        divField.setAttribute("data-droptop", parseInt(datatop, 10));
                    }
                    else if (itm.ReportItemType == enumItemType.Label) {
                        parts = id.split("Label_")
                        //fsLabel.FontName = itm.CaptionFontName;
                        //fsLabel.FontStyle = itm.CaptionFontStyle;
                        //fsLabel.FontSize = itm.CaptionFontSize;
                        //fsLabel.Underline = itm.CaptionUnderline;
                        //fsLabel.Strikeout = itm.CaptionStrikeout;
                        //fsLabel.ColorName = itm.CaptionForeColor;
                        //if (itm.CaptionForeColor.startsWith("#"))
                        //   fsLabel.ColorHex = itm.CaptionForeColor;
                        //else
                        //    fsLabel.ColorHex = toHex(itm.CaptionForeColor);

                        //dsLabel.BorderStyle = itm.CaptionBorderStyle;
                        //dsLabel.BorderWidth = itm.CaptionBorderWidth;
                        //dsLabel.BackColorName=itm.CaptionBackColor;
                        //if (itm.CaptionBackColor.startsWith("#"))
                        //    dsLabel.BackColorHex = itm.CaptionBackColor;
                        //else
                        //    dsLabel.BackColorHex = toHex(itm.CaptionBackColor);

                        //dsLabel.BorderColorName = itm.CaptionBorderColor;
                        //if (itm.CaptionBorderColor.startsWith("#"))
                        //    dsLabel.BorderColorHex = itm.CaptionBorderColor;
                        //else
                        //    dsLabel.BackColorHex = toHex(itm.CaptionBorderColor);


                        div = createLabel(parts[1]);
                        divDrop.appendChild(div);

                        if (hasHeightWidth) {
                            div.style.height = height + "px";
                            div.style.width = width + "px";
                        }

                        fieldno = divDrop.children.length - 1;
                        div.setAttribute("data-fieldindex", fieldno);
                        div.setAttribute("data-textalign", itm.TextAlign);

                        div.style.position = "absolute";
                        div.style.left = left + "px";
                        div.style.top = top + "px";

                        divCaption = div.children[0];
                        divCaption.style.textAlign = itm.TextAlign.toLowerCase();
                        divCaption.textContent = caption;
                    }
                    if (div != null) {
                        div.setAttribute("data-dropleft", parseInt(left, 10));
                        div.setAttribute("data-droptop", parseInt(top, 10));
                        div.setAttribute("data-dropwidth", parseInt(width, 10));
                        div.setAttribute("data-dropheight", parseInt(height, 10));
                    }
                }
                else if (section == "Header") {
                    if (itm.ReportItemType == enumItemType.Label) {
                        parts = id.split("Label_")
                        //fsLabel.FontName = itm.CaptionFontName;
                        //fsLabel.FontStyle = itm.CaptionFontStyle;
                        //fsLabel.FontSize = itm.CaptionFontSize;
                        //fsLabel.Underline = itm.CaptionUnderline;
                        //fsLabel.Strikeout = itm.CaptionStrikeout;
                        //fsLabel.ColorName = itm.CaptionForeColor;
                        //if (itm.CaptionForeColor.startsWith("#"))
                        //    fsLabel.ColorHex = itm.CaptionForeColor;
                        //else
                        //    fsLabel.ColorHex = toHex(itm.CaptionForeColor);
                        div = createLabel(parts[1]);
                        divHeader.appendChild(div);

                        if (hasHeightWidth) {
                            div.style.height = height + "px";
                            div.style.width = width + "px";
                        }

                        fieldno = divHeader.children.length;
                        div.setAttribute("data-fieldindex", fieldno);
                        div.setAttribute("data-textalign", itm.TextAlign);

                        div.style.position = "absolute";
                        div.style.left = left + "px";
                        div.style.top = top + "px";
                        divCaption = div.children[0];
                        divCaption.style.textAlign = itm.TextAlign.toLowerCase();
                        divCaption.textContent = caption;

                    }
                    else {
                        parts = id.split("_")

                        if (itm.ReportItemType == enumItemType.Image) {
                            dsImageFieldSettings.ImageWidth = itm.ImageWidth;
                            dsImageFieldSettings.ImageHeight = itm.ImageHeight;
                            dsImageFieldSettings.ImageData = itm.ImageData;
                            dsImageFieldSettings.ImagePath = itm.ImagePath;
                            dsImageFieldSettings.BorderStyle = itm.BorderStyle;
                            dsImageFieldSettings.BorderWidth = itm.BorderWidth;
                            dsImageFieldSettings.BorderColorName = itm.BorderColor;
                            if (itm.BorderColor.startsWith("#"))
                                dsImageFieldSettings.BorderColorHex = itm.BorderColor;
                            else
                                dsImageFieldSettings.BorderColorHex = toHex(itm.BorderColor);
                        }

                        div = createSpecialField(parts[0], caption, parts[1]);

                        // assign settings objects back to the default settings

                        // copy fs to fsDetail
                        fsDetail = Object.assign({}, fs);
                        // copy ds to dsDetail
                        dsDetail = Object.assign({}, ds);
                        // copy ifs to dsImageFieldSettings
                        dsImageFieldSettings = Object.assign({}, ifs)

                        div.setAttribute("data-itemtype", parts[0]);
                        div.setAttribute("data-textalign", itm.TextAlign);

                        divHeader.appendChild(div);

                        if (hasHeightWidth) {
                            div.style.height = height + "px";
                            div.style.width = width + "px";
                        }

                        fieldno = divHeader.children.length;
                        div.setAttribute("data-fieldindex", fieldno);
                        div.style.textAlign = itm.TextAlign.toLowerCase();

                        div.style.position = "absolute";
                        div.style.left = left + "px";
                        div.style.top = top + "px";
                    }
                    if (div != null) {
                        div.setAttribute("data-dropleft", parseInt(left, 10));
                        div.setAttribute("data-droptop", parseInt(top, 10));
                        div.setAttribute("data-dropwidth", parseInt(width, 10));
                        div.setAttribute("data-dropheight", parseInt(height, 10));
                    }
                }
                else if (section == "Footer") {

                    if (itm.ReportItemType == enumItemType.Label) {
                        parts = id.split("Label_")
                        //fsLabel.FontName = itm.CaptionFontName;
                        //fsLabel.FontStyle = itm.CaptionFontStyle;
                        //fsLabel.FontSize = itm.CaptionFontSize;
                        //fsLabel.Underline = itm.CaptionUnderline;
                        //fsLabel.Strikeout = itm.CaptionStrikeout;
                        //fsLabel.ColorName = itm.CaptionForeColor;
                        //if (itm.CaptionForeColor.startsWith("#"))
                        //    fsLabel.ColorHex = itm.CaptionForeColor;
                        //else
                        //    fsLabel.ColorHex = toHex(itm.CaptionForeColor);

                        div = createLabel(parts[1]);
                        divFooter.appendChild(div);

                        if (hasHeightWidth) {
                            div.style.height = height + "px";
                            div.style.width = width + "px";
                        }

                        fieldno = divFooter.children.length;
                        div.setAttribute("data-fieldindex", fieldno);
                        div.setAttribute("data-textalign", itm.TextAlign);

                        div.style.position = "absolute";
                        div.style.left = left + "px";
                        div.style.top = top + "px";
                        divCaption = div.children[0];
                        divCaption.style.textAlign = itm.TextAlign.toLowerCase();
                        divCaption.textContent = caption;
                    }
                    else {
                        parts = id.split("_")
                        //fsDetail.FontName = itm.FontName;
                        //fsDetail.FontStyle = itm.FontStyle;
                        //fsDetail.FontSize = itm.FontSize;
                        //fsDetail.Underline = itm.Underline;
                        //fsDetail.Strikeout = itm.Strikeout;
                        //fsDetail.ColorName = itm.ForeColor;
                        //if (itm.ForeColor.startsWith("#"))
                        //    fsDetail.ColorHex = itm.ForeColor;
                        //else
                        //    fsDetail.ColorHex = toHex(itm.ForeColor);

                        if (itm.ReportItemType == enumItemType.Image) {
                            dsImageFieldSettings.ImageWidth = itm.ImageWidth;
                            dsImageFieldSettings.ImageHeight = itm.ImageHeight;
                            dsImageFieldSettings.ImageData = itm.ImageData;
                            dsImageFieldSettings.ImagePath = itm.ImagePath;
                            dsImageFieldSettings.BorderStyle = itm.BorderStyle;
                            dsImageFieldSettings.BorderWidth = itm.BorderWidth;
                            dsImageFieldSettings.BorderColorName = itm.BorderColor;
                            if (itm.BorderColor.startsWith("#"))
                                dsImageFieldSettings.BorderColorHex = itm.BorderColor;
                            else
                                dsImageFieldSettings.BorderColorHex = toHex(itm.BorderColor);
                        }

                        div = createSpecialField(parts[0], caption, parts[1]);
                        // assign settings objects back to the default settings

                        // copy fs to fsDetail
                        fsDetail = Object.assign({}, fs);
                        // copy ds to dsDetail
                        dsDetail = Object.assign({}, ds);
                        // copy ifs to dsImageFieldSettings
                        dsImageFieldSettings = Object.assign({}, ifs)

                        div.setAttribute("data-itemtype", parts[0]);

                        divFooter.appendChild(div);

                        if (hasHeightWidth) {
                            div.style.height = height + "px";
                            div.style.width = width + "px";
                        }

                        fieldno = divFooter.children.length;
                        div.setAttribute("data-fieldindex", fieldno);
                        div.setAttribute("data-textalign", itm.TextAlign);

                        div.style.textAlign = itm.TextAlign.toLowerCase();
                        div.style.position = "absolute";
                        div.style.left = left + "px";
                        div.style.top = top + "px";
                    }
                    if (div != null) {
                        div.setAttribute("data-dropleft", parseInt(left, 10));
                        div.setAttribute("data-droptop", parseInt(top, 10));
                        div.setAttribute("data-dropwidth", parseInt(width, 10));
                        div.setAttribute("data-dropheight", parseInt(height, 10));
                    }
                }
                else if (section == "Groups") {
                    div = createGroupItem(itm);

                    //div.style.height = itm.Height + "px";


                    // copy fs to fsGroupFieldSettings
                   fsGroupFieldSettings = Object.assign({}, fs);
                    // copy ds to dsGroupFieldSettings
                   dsGroupFieldSettings = Object.assign({}, ds);

                   div.setAttribute("data-itemtype", "Group");
                   div.setAttribute("data-fieldindex", itm.ItemOrder);
                   div.setAttribute("data-textalign", itm.TextAlign);

                    var groupTop = 3;

                    div.style.textAlign = itm.TextAlign.toLowerCase();

                    var len  = divGroup.children.length;

                    if (len > 0) {
                        for (var j = 0; j < len; j++) {
                            groupTop = groupTop + parseInt(divGroup.children[j].style.height,10) + 10;
                        }

                    }
                    div.style.position = "absolute";
                    div.style.left = "3px";
                    div.style.top = groupTop + "px";

                    divGroup.appendChild(div);


                }
            }
            setColWidthBorders(divDrop);
        }

    }
}

function doesFontExist(fontName) {
    // creating our in-memory Canvas element where the magic happens
    var canvas = document.createElement("canvas");
    var context = canvas.getContext("2d");

    // the text whose final pixel size I want to measure
    var text = "abcdefghijklmnopqrstuvwxyz0123456789";

    // specifying the baseline font
    context.font = "72px monospace";

    // checking the size of the baseline text
    var baselineSize = context.measureText(text).width;

    // specifying the font whose existence we want to check
    context.font = "72px '" + fontName + "', monospace";

    // checking the size of the font we want to check
    var newSize = context.measureText(text).width;

    // removing the Canvas element we created
    canvas = null;

    //
    // If the size of the two text instances is the same, the font does not exist because it is being rendered
    // using the default sans-serif font
    //
    if (newSize == baselineSize) {
        return false;
    } else {
        return true;
    }
}

function loadFonts(fontdata) {
    fontData = fontData || new FontData();
    if (fontdata != null) {
        fontdata = fontdata.replace(/\\/g, "");
        FontData.ParseJSON(JSON.parse(fontdata), fontData);

        for (var i = 0; i < fontData.Items.length; i++) {
            var fi = fontData.Items[i];
            var fontExists = doesFontExist(fi.Name);
            if (fontExists)
                availableFonts.push(fi.Name);
        }
    }

}

function loadReportView() {
    if (!isLoaded) {
        reportView = reportView || new ReportView();
        var hdnReportView = document.getElementById("hdnReportView");
        var fontdata = document.getElementById("hdnFontData").value;

        populateReportView(hdnReportView.value);
        isLoaded = true;
        loadReportDisplay();
        changeDisplayLines();
        loadFonts(fontdata);
        loadColorLists();
    }
}

function showSpinner() {
    setTimeout(function () { document.getElementById("spinner").style.display = ""; document.images["imgSpinner"].src = "Controls/Images/WaitImage2.gif"; }, 200);
}

function onColorLeave(e) {
    targ=e.target;
    if (targ != void 0 && targ.id.startsWith("div")) {
        var td = targ.parentElement;
        var tr = td.parentElement;
        var divText = tr.cells[1].children[0];
        divText.className="divColorText";
    }
}

function onColorNameClick(e) {
    onColorClick(e);
}

function onColorClick(e) {
    var targ=e.target;
    if (targ != void 0 && targ.id.startsWith("div")) {
        var colorName=targ.dataset.colorname;
        var colorHex = toHex(colorName);
        var tblColorList = targ.dataset.tblcolorname;
        var elTblColorList;
        var tag;
        var bShowSample = true
        var divShowColor;  //= document.getElementById("divShowColor");
        var divColorName; // = document.getElementById("divColorName")
        var divColorList; // = document.getElementById("divColorList");
        //var tblColorList  = document.getElementById("tblColorList");

        if (tblColorList != void 0) elTblColorList = document.getElementById(tblColorList);
        if (elTblColorList !=  void 0 && elTblColorList != "undefined") {
            if (elTblColorList.hasAttribute("tag"))
                 tag = elTblColorList.getAttribute("tag");

            switch (tblColorList) {
                case "tblColorList":
                    if (tag == "divChooseColor") {
                        divShowColor = document.getElementById("divShowColor");
                        divColorName = document.getElementById("divColorName");
                        //divColorList  = document.getElementById("divColorList");
                    }
                    else {
                        divShowColor = document.getElementById("divShowFieldTextColor");
                        divColorName = document.getElementById("divFieldTextColorName");
                        bShowSample = false;
                    }
                    divColorList = document.getElementById("divColorList");
                    break;
                case "tblBackColorList":
                    if (tag == "divChooseBackColor") {
                        divShowColor = document.getElementById("divShowBackColor");
                        divColorName = document.getElementById("divBackColorName");
                    }
                    else {
                        divShowColor = document.getElementById("divHeaderFooterShowBackColor");
                        divColorName = document.getElementById("divHeaderFooterBackColorName");
                        bShowSample = false;
                    }
                    divColorList  = document.getElementById("divBackColorList");
                    break;
                case "tblBorderColorList":
                    if (tag == "divChooseBorderColor") {
                        divShowColor = document.getElementById("divShowBorderColor");
                        divColorName = document.getElementById("divBorderColorName");
                    }
                    else if (tag == "divImageFieldChooseBorderColor") {
                        divShowColor = document.getElementById("divImageFieldShowBorderColor");
                        divColorName = document.getElementById("divImageFieldBorderColorName");
                    }
                    else {
                        divShowColor = document.getElementById("divHeaderFooterShowBorderColor");
                        divColorName = document.getElementById("divHeaderFooterBorderColorName");
                        bShowSample = false;
                    }
                    divColorList  = document.getElementById("divBorderColorList");
                    break;
            }
            divColorList.style.display="none";
            divColorName.textContent = colorName;
            divShowColor.style.backgroundColor=colorName;
            if (bShowSample) {
                if (tag == "divImageFieldChooseBorderColor") {
                    fieldSettings.BorderColorName = colorName;
                    if (colorName.startsWith('#'))
                        fieldSettings.BorderColorHex = colorName
                    else
                        fieldSettings.BorderColorHex = colorHex;
                    showSampleImage();
                }
                else
                    showSample();

            }
                
        }
        //switch (tblColorList) {
        //    case "tblColorList":
        //        divShowColor = document.getElementById("divShowColor");
        //        divColorName = document.getElementById("divColorName")
        //        divColorList  = document.getElementById("divColorList");
        //        break;
        //    case "tblBackColorList":
        //        divShowColor = document.getElementById("divShowBackColor");
        //        divColorName = document.getElementById("divBackColorName")
        //        divColorList  = document.getElementById("divBackColorList");
        //        break;
        //    case "tblBorderColorList":
        //        divShowColor = document.getElementById("divShowBorderColor");
        //        divColorName = document.getElementById("divBorderColorName")
        //        divColorList  = document.getElementById("divBorderColorList");
        //        break;
        //}
        //divColorList.style.display="none";
        //divColorName.textContent = colorName;
        //divShowColor.style.backgroundColor=colorName;
        //showSample();
    }
    return false;
}

function onColorHover(e) {
    targ=e.target;
    if (targ != void 0 && targ.id.startsWith("div")) {
        var td = targ.parentElement;
        var tr = td.parentElement;
        var divText = tr.cells[1].children[0];
        divText.className="ColorSelected";

    }
}

function hideList(e) {
    var divColorList = e.target; //document.getElementById("divColorList");
    if (e.relatedTarget == null || e.relatedTarget.className != "divColorText")
      divColorList.style.display="none";
}

function loadColWidthSizer() {
    var reportType = reportView.ReportTemplate;
    var divDrop = document.getElementById("divDrop");
    var divColWidthBody = document.getElementById("divTabularWidthBody");
    var div;
    var divColSizer;
    if (reportType == enumReportTemplate.Tabular) {
        clearFields(divColWidthBody);
        for (var i = 0; i < divDrop.children.length; i++) {
           
            if (divDrop.children[i].id != "DropLine") {
                div = createSizableColDiv(divDrop.children[i]);
                divColSizer = createResizer();
                div.appendChild(divColSizer);
                div.addEventListener('mouseover',onColMouseOver);
                div.addEventListener('mouseout',onColMouseOut);
                divColWidthBody.appendChild(div);
            }
                
        }
        setColWidthBorders(divColWidthBody);
    }
}
function loadColorList(tblColorName) {
    var tblColorList = document.getElementById(tblColorName);
    var endingID = "Color"

    if (tblColorName.indexOf("BackColor") > -1)
        endingID = "BackColor"
    else if (tblColorName.indexOf("BorderColor") > -1)
        endingID = "BorderColor"

    tblColorList.setAttribute("data-selectedcolor","black");

    var j=0;
    Object.entries(wordToHex).forEach(([key,value]) => {
        var name = key;
        var hex = value;

        var row = tblColorList.insertRow(j);
        row.setAttribute("data-colorname",name);
        row.setAttribute("data-selected",false);
        row.style.padding = "1px";
        j=parseInt(j+1,10);
        var td = row.insertCell(0);
        //td.id="tdColor_"+ name;
        td.id="td" + endingID + "_" + name;
        var div = document.createElement("div");
        //div.id="div" + name + "_" + (j-1).toString();
        //div.id="divColor_" + name;
        div.id="div" + endingID + "_" + name;
        div.className="divColor";
        div.style.backgroundColor=name;

        div.setAttribute("data-colorname",name);
        div.setAttribute("data-selected",false);
        div.setAttribute("data-tblColorName",tblColorName);
        div.addEventListener("mouseover",onColorHover);
        div.addEventListener("mouseleave",onColorLeave);
        div.addEventListener("click",onColorClick);
        td.appendChild(div);
        
        td=row.insertCell(1);
        //td.id="tdColorText_" + name;
        td.id="td" + endingID + "Text_" + name;
        div=document.createElement("div");
        //div.id="divColorText_" + name;
        div.id = "div"  + endingID + "Text_" + name;
        //div.id="div" + name + "text_" + (j-1).toString();
        div.className = "divColorText";
        div.textContent=name;

        div.setAttribute("data-colorname",name);
        div.addEventListener("click",onColorNameClick);
        div.setAttribute("data-selected",false);
        div.setAttribute("data-tblColorName",tblColorName);
        //div.tabIndex="0";
        td.appendChild(div);
    });
}
function loadColorLists() {
    loadColorList("tblColorList");
    loadColorList("tblBackColorList");
    loadColorList("tblBorderColorList");
}

function showSection(section) {
    var lblSection = document.getElementById("lblSection");
    var hdnSection = document.getElementById("hdnSection");
    var divDetails = document.getElementById("divDrop");
    var divHeader = document.getElementById("divHeaderDisplay");
    var divFooter = document.getElementById("divFooterDisplay");
    var divGroup = document.getElementById("divGroup");
    var divSpecialFieldList = document.getElementById("divSpecialFieldList");
    var divFieldList = document.getElementById("divFieldList");
    var divGroupList = document.getElementById("divGroupList");
    var divSectionList = document.getElementById("divSectionList");
    var lstSections = document.getElementById("lstSections_InnerList");
    var listContainer = document.getElementById("lstSections");
    var hfSelectedIndex = listContainer.children[0]  //  hidden field inside draglist that stores current selected index
    var nSections = lstSections.children.length;

    if (currentSection != section) {
        switch (section) {
            case "divDrop":
                currentSection="divDrop";
                hdnSection.value="Details";
                lblSection.textContent="Details";
                divHeader.style.display="none";
                divFooter.style.display="none";
                divDetails.style.display="";
                divSpecialFieldList.style.display="none";
                divFieldList.style.display = "";
                divGroupList.style.display = "none";
                divGroup.style.display = "none";
                divSectionList.style.marginRight="38%";
                lstSections.children[0].className = "selected"
                listContainer.setAttribute("data-selectedindex","0");
                hfSelectedIndex.value = "0"
                lstSections.children[1].className = "li";
                lstSections.children[2].className = "li";
                if (nSections ==4) lstSections.children[3].className = "li";
                break;
            case "divHeader":
                currentSection="divHeader";
                hdnSection.value="Report Header";
                lblSection.textContent="Report Header";
                divHeader.style.display="";
                divFooter.style.display="none";
                divDetails.style.display="none";
                divSpecialFieldList.style.display="";
                divFieldList.style.display = "none";
                divGroupList.style.display = "none";
                divGroup.style.display = "none";
                lstSections.children[1].className = "selected"
                listContainer.setAttribute("data-selectedindex","1");
                hfSelectedIndex.value = "1"
                lstSections.children[0].className = "li";
                lstSections.children[2].className = "li";
                if (nSections == 4) lstSections.children[3].className = "li";

                 break;
            case "divFooter":
                currentSection="divFooter";
                hdnSection.value="Report Footer";
                lblSection.textContent="Report Footer";
                divHeader.style.display="none";
                divFooter.style.display="";
                divDetails.style.display="none";
                divSpecialFieldList.style.display="";
                divFieldList.style.display = "none";
                divGroupList.style.display = "none";
                divGroup.style.display = "none";
                lstSections.children[2].className = "selected"
                listContainer.setAttribute("data-selectedindex","2");
                hfSelectedIndex.value = "2"
                lstSections.children[0].className = "li";
                lstSections.children[1].className = "li";
                if (nSections == 4) lstSections.children[3].className = "li";
                break;
            case "divGroup":
                currentSection = "divGroup";
                hdnSection.value = "Report Group";
                lblSection.textContent = "Report Group";
                divHeader.style.display = "none";
                divFooter.style.display = "none";
                divDetails.style.display = "none";
                divSpecialFieldList.style.display = "none";
                divFieldList.style.display = "none";
                divGroupList.style.display = "";
                divGroup.style.display = "";
                lstSections.children[2].className = "selected"
                listContainer.setAttribute("data-selectedindex", "2");
                hfSelectedIndex.value = "2"
                lstSections.children[0].className = "li";
                lstSections.children[1].className = "li";
                break;

        }
    }
}
function OnGroupListClicked() {
    var e = event;
    var curtrgt = e.currentTarget;
    var trgt = e.target
    var itemText

    if (trgt.id.startsWith("lstGroups_li") && curtrgt.id == "divGroupList") {
        itemText = trgt.innerText;

    }
}
function OnListClicked() {
    var e = event || window.event;
    var curtrgt = e.currentTarget;
    var trgt = e.target || e.srcElement;
    var itm = document.getElementById(trgt.id);
    var itemText = itm.innerText;

    if (trgt.id.startsWith("lstSections_li") && curtrgt.id == "divSectionList" ) {
        var di_id = itm.getAttribute("dragitemID");
        var lblSection = document.getElementById("lblSection");
        var hdnSection = document.getElementById("hdnSection");
        var divDetails = document.getElementById("divDrop");
        var divHeader = document.getElementById("divHeaderDisplay");
        var divFooter = document.getElementById("divFooterDisplay");
        var divSpecialFieldList = document.getElementById("divSpecialFieldList");
        var divFieldList = document.getElementById("divFieldList");
        var divSectionList = document.getElementById("divSectionList");
        var divGroup = document.getElementById("divGroup");
        var divGroupList = document.getElementById('divGroupList');

        switch (di_id) {
            case "Section_Details":
                currentSection="divDrop";
                hdnSection.value="Details";
                lblSection.textContent="Details";
                divHeader.style.display="none";
                divFooter.style.display="none";
                divDetails.style.display = "";
                divGroup.style.display = "none";
                divSpecialFieldList.style.display="none";
                divFieldList.style.display = "";
                divGroupList.style.display = "none";
                divSectionList.style.marginRight="38%";
                break;
            case "Section_ReportHeader":
                currentSection="divHeader";
                hdnSection.value="Report Header";
                lblSection.textContent="Report Header";
                divHeader.style.display="";
                divFooter.style.display="none";
                divDetails.style.display = "none";
                divGroup.style.display = "none";
                divSpecialFieldList.style.display="";
                divFieldList.style.display = "none";
                divGroupList.style.display = "none";
                //divSectionList.style.marginRight="50%";
                break;
            case "Section_ReportFooter":
                currentSection="divFooter";
                hdnSection.value="Report Footer";
                lblSection.textContent="Report Footer";
                divHeader.style.display="none";
                divFooter.style.display="";
                divDetails.style.display = "none";
                divGroup.style.display = "none";
                divSpecialFieldList.style.display="";
                divFieldList.style.display = "none";
                divGroupList.style.display = "none";
                //divSectionList.style.marginRight="50%";
                break;
            case "Section_ReportGroups":
                currentSection = "divGroup";
                hdnSection.value = "Report Group";
                lblSection.textContent = "Report Group";
                divHeader.style.display = "none";
                divFooter.style.display = "none";
                divDetails.style.display = "none";
                divGroup.style.display = "";
                divSpecialFieldList.style.display = "none";
                divFieldList.style.display = "none";
                divGroupList.style.display = "";
                //divSectionList.style.marginRight="50%";
                break;
        }
    }

}

function OnListChanged(ctlId) {
    var e = event || window.event;
    var trgt = e.target || e.srcElement; 


    if (trgt.id == ctlId) {
        var sel = parseInt(trgt.dataset.selectedindex, 10);
        if (sel != void 0) {
            var selidx = parseInt(sel + 1, 10);
            var id = trgt.id + "_li" + selidx;
            var itm=document.getElementById(id);
            var di_id = itm.getAttribute("dragitemID");
            if (ctlId == "lstSections") {
                //var lblHeader = document.getElementById("lblHeader");
                var lblSection = document.getElementById("lblSection");
                //var reportTitle = document.getElementById("hdnReportTitle").value;
                //var reportTemplate = (reportView.ReportTemplate==enumReportTemplate.FreeForm) ? "FreeForm":"Tabular";
                var hdnSection = document.getElementById("hdnSection");
                var divDetails = document.getElementById("divDrop");
                var divHeader = document.getElementById("divHeaderDisplay");
                var divFooter = document.getElementById("divFooterDisplay");
                var divSpecialFieldList = document.getElementById("divSpecialFieldList");
                var divFieldList = document.getElementById("divFieldList");
                var divSectionList = document.getElementById("divSectionList");

                switch (di_id) {
                    case "Section_Details":
                        currentSection="divDrop";
                        hdnSection.value="Details";
                        lblSection.textContent="Details";
                        divHeader.style.display="none";
                        divFooter.style.display="none";
                        divDetails.style.display="";
                        divSpecialFieldList.style.display="none";
                        divFieldList.style.display="";
                        divSectionList.style.marginRight="38%";
                         break;
                    case "Section_ReportHeader":
                        currentSection="divHeader";
                        hdnSection.value="Report Header";
                        lblSection.textContent="Report Header";
                        divHeader.style.display="";
                        divFooter.style.display="none";
                        divDetails.style.display="none";
                        divSpecialFieldList.style.display="";
                        divFieldList.style.display="none";
                         //divSectionList.style.marginRight="50%";
                        break;
                    case "Section_ReportFooter":
                        currentSection="divFooter";
                        hdnSection.value="Report Footer";
                        lblSection.textContent="Report Footer";
                        divHeader.style.display="none";
                        divFooter.style.display="";
                        divDetails.style.display="none";
                        divSpecialFieldList.style.display="";
                        divFieldList.style.display="none";
                         //divSectionList.style.marginRight="50%";
                        break;
                }
            }
        }
    }
}

function OnListDblClicked(ctlId) {
    var e = event;
    var trgt = e.target;
    if (trgt.id == ctlId) {
        var selidx = parseInt(trgt.dataset.selectedindex, 10);
        var hdnReportID = document.getElementById("hdnReportID");
        if (selidx != void 0) {
            var sel = parseInt(selidx + 1, 10);
            var id = trgt.id + "_li" + sel;
            var itm=document.getElementById(id);
            var reportID = itm.getAttribute("dragitemID");
            reportView.ReportID = reportID;
            hdnReportID.value = reportID;
            __doPostBack(ctlId + "DoubleClick", selidx);
        }

    }

}

function allowDrop(ev) {
    //if (ev.dataTransfer.types.includes("allow_drop"))
    ev.preventDefault();
}

function allowHeaderFooterDrop(ev) {
    //if (ev.dataTransfer.types.includes("allow_drop"))
     ev.preventDefault();
}

function dragDiv(ev) {
    var clientRect = ev.currentTarget.getBoundingClientRect();
    var offsetX = parseInt(ev.offsetX,10);
    var offsetY = parseInt(ev.offsetY, 10);

    ev.dataTransfer.dropEffect = 'move';
    ev.dataTransfer.setData("text", ev.target.id + "," + offsetX + "," + offsetY);
    //ev.dataTransfer.setData("allow_drop","");
}

function getCaptionDiv(divField) {
    if (divField != void 0 && divField.id.startsWith("div_")) {
        return divField.children[0];
    }
    return null;
}

function deleteField(divField) {
    var reportType = reportView.ReportTemplate;

    if (divField != void 0 && divField.id.startsWith("div")) {
        var divDrop = document.getElementById(currentSection);
        if (divDrop != void 0) {
            divDrop.removeChild(divField);
            reindexItems(divDrop);
            if (divField.id.startsWith("div_") && reportType == enumReportTemplate.Tabular) {
                resetItemsSize(divDrop);
            }
        }

    }
}


function onGroupItemKeydown(e) {
    var trgt = e.target;
    var keyCode = e.keyCode;

    if (trgt.id.startsWith("divGroupItem_")) {
        var top = trgt.style.top;
        var divGroupItem = document.getElementById(trgt.id);

        if (keyCode == 9 || keyCode == 27 || keyCode == 38 || keyCode == 40) {
            e.preventDefault();
            e.stopPropagation();
            switch (keyCode) {
                case 9: //tab key and escape -- turns off arrow key moving and loses focus;
                case 27: //escape
                    divGroupItem.blur();
                    break;
                case 38: //up
                    top = parseInt(top, 10) - 1;
                    divGroupItem.style.top = top + "px";
                    divGroupItem.setAttribute("data-droptop", top);
                    break;
                case 40: //down
                    top = parseInt(top, 10) + 1;
                    divGroupItem.style.top = top + "px";
                    divGroupItem.setAttribute("data-droptop", top);
                    break;
            }
        }
    }

}
function onLabelKeydown(e) {
    var trgt = e.target;
    var keyCode = e.keyCode;

    if (trgt.id.startsWith("divLabel_")) {
        var top = trgt.style.top;
        var left = trgt.style.left;
        var divLabel = document.getElementById(trgt.id);

        if (keyCode==9 || keyCode== 27 || keyCode == 37 || keyCode == 38 || keyCode == 39 || keyCode == 40) {
            e.preventDefault();
            e.stopPropagation();
            switch (keyCode) {
                case 9: //tab key and escape -- turns off arrow key moving and loses focus;
                case 27: //escape
                    divLabel.blur();
                    break;
                case 37: //left
                    left = parseInt(left,10)-1;
                    divLabel.style.left = left + "px";
                    divLabel.setAttribute("data-dropleft", left);
                    break;
                case 38: //up
                    top = parseInt(top, 10) - 1;
                    divLabel.style.top = top + "px";
                    divLabel.setAttribute("data-droptop", top);
                    break;
                case 39: //right
                    left = parseInt(left, 10) + 1;
                    divLabel.style.left = left + "px";
                    divLabel.setAttribute("data-dropleft", left);
                    break;
                case 40: //down
                    top = parseInt(top, 10) + 1;
                    divLabel.style.top = top + "px";
                    divLabel.setAttribute("data-droptop", top);
                    break;
            }
        }
    }
}

function onFieldKeydown(e) {
    var trgt = e.target;
    var keyCode = e.keyCode;

    if (trgt.id.startsWith("div")) {
        var top = trgt.style.top;
        var left = trgt.style.left;
        var div = document.getElementById(trgt.id);
        var divParent = div.parentElement;
        var divCaption;
        var divField;
        var hasChildren = false;
        if (trgt.id.startsWith("div_")) {
            divCaption = div.children[0];
            divField = div.children[1];
            hasChildren=true;
        }
        if (keyCode==9 || keyCode== 27 || keyCode == 37 || keyCode == 38 || keyCode == 39 || keyCode == 40) {
            e.preventDefault();
            e.stopPropagation();
            switch (keyCode) {
                case 9: //tab key and escape -- turns off arrow key moving and loses focus;
                case 27: //escape
                    div.blur();
                    break;
                case 37: //left
                    left = parseInt(left,10)-1;
                    div.style.left = left + "px";
                    div.setAttribute("data-dropleft", left);
                    if (hasChildren) {
                        left=parseInt(divCaption.dataset.dropleft,10)-1;
                        divCaption.setAttribute("data-dropleft", left);
                        left=parseInt(divField.dataset.dropleft,10)-1;
                        divField.setAttribute("data-dropleft", left);
                    }
                    break;
                case 38: //up
                    top = parseInt(top, 10) - 1;
                    div.style.top = top + "px";
                    div.setAttribute("data-droptop", top);
                    if (hasChildren) {
                        top=parseInt(divCaption.dataset.droptop,10)-1;
                        divCaption.setAttribute("data-droptop", top);
                        top=parseInt(divField.dataset.droptop,10)-1;
                        divField.setAttribute("data-droptop", top);
                    }
                    break;
                case 39: //right
                    left = parseInt(left, 10) + 1;
                    div.style.left = left + "px";
                    div.setAttribute("data-dropleft", left);
                    if (hasChildren) {
                        left=parseInt(divCaption.dataset.dropleft,10)+1;
                        divCaption.setAttribute("data-dropleft", left);
                        left=parseInt(divField.dataset.dropleft,10)+1;
                        divField.setAttribute("data-dropleft", left);
                    }
                    break;
                case 40: //down
                    top = parseInt(top, 10) + 1;
                    div.style.top = top + "px";
                    div.setAttribute("data-droptop", top);
                    if (hasChildren) {
                        top=parseInt(divCaption.dataset.droptop,10)+1;
                        divCaption.setAttribute("data-droptop", top);
                        top=parseInt(divField.dataset.droptop,10)+1;
                        divField.setAttribute("data-droptop", top);
                    }
                    break;

            }
        }

    }
}

function onGroupItemFocus(e) {
    var trgt = e.target;

    if (trgt.id.startsWith("divGroupItem_")) {
        var divGroupItem = document.getElementById(trgt.id);
        fsGroupFieldSettings = setFontSettings(divGroupItem);
       dsGroupFieldSettings = setDisplaySettings(divGroupItem);
        divGroupItem.style.borderStyle = "dashed";
        divGroupItem.style.color = "white";
        divGroupItem.style.backgroundColor = "highlight";

        divGroupItem.style.outline = "none";
    }
}
function onLabelFocus(e) {
    var trgt = e.target;

    if (trgt.id.startsWith("divLabel_")) {
        var divLabel = document.getElementById(trgt.id);
        fsLabel = setFontSettings(divLabel);
        dsLabel = setDisplaySettings(divLabel);
        divLabel.style.borderStyle = "dashed";
        divLabel.style.color = "white";
        divLabel.style.backgroundColor = "highlight";

        divLabel.style.outline = "none";
    }
}

function onFieldFocus(e) {
    var trgt = e.target;
    if (trgt.id.startsWith("div")) {
        var div = document.getElementById(trgt.id);
        //trgt.style.borderStyle = "dotted";
        if (trgt.id.startsWith("div_")) {
            var divCaption = div.children[0];
            var divField = div.children[1];
            dsCaption = setDisplaySettings(divCaption);
            dsDetail = setDisplaySettings(divField);
            divCaption.style.borderStyle = "dashed";
            divCaption.style.color = "white";
            divCaption.style.backgroundColor = "highlight" //"#e6eefa";
            divField.style.borderStyle = "dashed";
            divField.style.color = "white";
            divField.style.backgroundColor = "highlight";
        }
        else {
            fsLabel = setFontSettings(div);
            dsLabel = setDisplaySettings(div);
            div.style.borderStyle = "dashed";
            div.style.color = "white";
            div.style.backgroundColor = "highlight";
        }
        trgt.style.outline = "none";
    }
}

function onGroupItemBlur(e) {
    var trgt = e.target;

    if (trgt.id.startsWith("divGroupItem_")) {
        var divGroupItem = document.getElementById(trgt.id);
        doSettings(divGroupItem, fsGroupFieldSettings);
        doDisplaySettings(divGroupItem, dsGroupFieldSettings);

        divGroupItem.removeEventListener("keydown", onGroupItemKeydown);
        divGroupItem.removeEventListener("focus", onGroupItemFocus);
        divGroupItem.removeEventListener("blur", onGroupItemBlur);

        divGroupItem.style.outline = "none";
    }

}

function onLabelBlur(e) {
    var trgt = e.target;

    if (trgt.id.startsWith("divLabel_")) {
        var divLabel = document.getElementById(trgt.id);
        doSettings(divLabel,fsLabel);
        doDisplaySettings(divLabel,dsLabel);
        //divLabel.style.borderStyle="solid";
        //divLabel.style.color = "black";
        //divLabel.style.backgroundColor = "white";

        divLabel.removeEventListener("keydown", onLabelKeydown);
        divLabel.removeEventListener("focus", onLabelFocus);
        divLabel.removeEventListener("blur", onLabelBlur);

        divLabel.style.outline = "none";
    }
}

function onFieldBlur(e) {
    var trgt = e.target;
    if (trgt.id.startsWith("div")) {
        var div = document.getElementById(trgt.id);
        if (trgt.id.startsWith("div_")) {
            var divCaption = div.children[0];
            var divField = div.children[1];
            doSettings(divCaption,fsCaption);
            doDisplaySettings(divCaption,dsCaption);

            doSettings(divField,fsDetail);
            doDisplaySettings(divField,dsDetail);

            //divCaption.style.borderStyle = "solid";
            //divCaption.style.color = "black";
            //divCaption.style.backgroundColor = "lightyellow";
            //divField.style.borderStyle = "solid";
            //divField.style.color = "black";
            //divField.style.backgroundColor = "#ffffff";
        }
        else {
            doSettings(div,fsLabel);
            doDisplaySettings(div,dsLabel);
            //div.style.backgroundColor = "#ffffff";
            //div.style.borderStyle = "solid";
        }

        div.removeEventListener("keydown", onFieldKeydown);
        div.removeEventListener("focus", onFieldFocus);
        div.removeEventListener("blur", onFieldBlur);

        div.style.outline = "none";
    }
}

function moveWithArrowKeys(divField) {
    if (divField != void 0 && divField.id.startsWith("div")) {
        divField.addEventListener("keydown", onFieldKeydown);
        divField.addEventListener("focus", onFieldFocus);
        divField.addEventListener("blur", onFieldBlur);
        divField.focus();
    }
}

function moveLabelWithArrows(divLabel) {
   if (divLabel != void 0 & divLabel.id.startsWith("divLabel_")) {
       divLabel.addEventListener("keydown", onLabelKeydown);
       divLabel.addEventListener("focus", onLabelFocus);
       divLabel.addEventListener("blur", onLabelBlur);
       divLabel.focus();
    }
}

function moveGroupVertically(divGroupItem) {
    if (divGroupItem != void 0 & divGroupItem.id.startsWith("divGroupItem_")) {
        divGroupItem.addEventListener("keydown", onGroupItemKeydown);
        divGroupItem.addEventListener("focus", onGroupItemFocus);
        divGroupItem.addEventListener("blur", onGroupItemBlur);
        divGroupItem.focus();
    }
}

function deleteLabel(divLabel) {
    if (divLabel != void 0 && divLabel.id.startsWith("divLabel_")) {
        var divDrop = document.getElementById(currentSection);
        if (divDrop != void 0) divDrop.removeChild(divLabel);
    }
}

function clearFields(div) {
    if (div != void 0) {
        for (var i = div.children.length-1; i > -1; i--) {
            var divChild = div.children[i];
            if (divChild.id.startsWith("div"))
              div.removeChild(div.children[i]);
        }
    }
}

function changeOrientation(id) {
    var orientation = id.toLowerCase();
    if (reportView.Orientation != orientation) {
        reportView.Orientation = orientation;
        changeDisplayLines();
    }
}

function changeDisplayLines() {
    var orientation = reportView.Orientation;
    var DropLine = document.getElementById("DropLine");
    var divHeader = document.getElementById("divHeader");
    var divFooter = document.getElementById("divFooter");

    //var HeadVerticalLine = document.getElementById("HeadVerticalLine");
    //var HeadHorizontalLine = document.getElementById("HeadHorizontalLine");
    //var FootVerticalLine = document.getElementById("FootVerticalLine");
    //var FootHorizontalLine = document.getElementById("FootHorizontalLine");

    if (orientation == "portrait") {
        DropLine.style.left = "8in";
        divHeader.style.width= "8in";
        divFooter.style.width = "8in";
        //HeadVerticalLine.style.left = "8in";
        //HeadHorizontalLine.style.width="8in";
        //FootVerticalLine.style.left = "8in";
        //FootHorizontalLine.style.width="8in";
    }
    else {
        DropLine.style.left = "10.5in";
        divHeader.style.width = "10.5in";
        divFooter.style.width = "10.5in";
        //HeadVerticalLine.style.left = "10.5in";
        //HeadHorizontalLine.style.width="10.5in";
        //FootVerticalLine.style.left = "10.5in";
        //FootHorizontalLine.style.width="10.5in";
    }
}
function reindxList(list) {
    var li
    var listContainer = document.getElementById("lstFields");
    listContainer.setAttribute("dataset-selectedindex","-1");

    for (var i = 0; i < list.children.length; i++) {
        li = list.children[i];
        li.setAttribute("index", i);
        var id = "lstFields_li" + (i+1).toString(10);
        li.setAttribute("id",id);
        if (li.className=="selected")  {
            listContainer.setAttribute("dataset-selectedindex",i.toString(10));
        }
    }
}
function changeFieldList(template) {
    var reportType = reportView.ReportTemplate;
    var templateUpper = template.toUpperCase();
    var lstFields = document.getElementById("lstFields_InnerList");
    var listContainer = document.getElementById("lstFields");
    var itm;
    var liLabel;

    if (lstFields != void 0 && lstFields.children.length > 0) {
        if (templateUpper == "TABULAR" && reportType != enumReportTemplate.Tabular) {  // delete label item
            itm = lstFields.children[0];
            if (itm.innerText == "** Label **") {
                lstFields.removeChild(itm);
                reindxList(lstFields);
             }
        }
        else if (templateUpper == "FREEFORM" && reportType != enumReportTemplate.FreeForm) { // add label item
            itm = lstFields.children[0];
            if (itm.innerText != "** Label **") {
               liLabel = document.createElement("li");
               liLabel.innerHTML = "** Label **";
               liLabel.id = "lstFields_li1";
               liLabel.setAttribute("dragitemID","Label");
               liLabel.setAttribute("dragitemTag","0");
               liLabel.setAttribute("tabindex","-1");
               liLabel.setAttribute("onfocus","OnItemClick('lstFields');");
               liLabel.setAttribute("ondblclick","OnDblClick('lstFields');");
               liLabel.setAttribute("class","li");
               liLabel.setAttribute("draggable","true");
               liLabel.setAttribute("ondragstart","drag(event);");
               liLabel.setAttribute("index","0");
               liLabel.setAttribute("style",itm.getAttribute("style") );

               lstFields.insertBefore(liLabel,itm);
               reindxList(lstFields);
            }
        }
    }
}
function changeTemplate(template) {
    var reportType = reportView.ReportTemplate;
    var templateUpper = template.toUpperCase();
    var divDrop = document.getElementById("divDrop");
    var len=divDrop.children.length;

    reportView.ReportFieldLayout = "Block";

    changeFieldList(template);
    //var dropRect = divDrop.getBoundingClientRect();
    if (divDrop != void 0 && len> 0) {
        // since some fields maybe deleted, start from end
        for (var i = len-1; i > -1; i--) {
            var div = divDrop.children[i];
            if (div.id.startsWith("div_")) {
                var divCaption = div.children[0];
                var divField = div.children[1];
                var colwidth = (div.dataset.tabularcolwidth != void 0 && div.dataset.tabularcolwidth != "") ? div.dataset.tabularcolwidth:"1.5in";
                var caption = divCaption.textContent.split(":")[0];
                //var divRect = div.getBoundingClientRect();
                //var left = parseInt(divRect.left-dropRect.left,10);
                //var top = parseInt(divRect.top - dropRect.top, 10);
                if (templateUpper == "TABULAR" && reportType != enumReportTemplate.Tabular) {
                    div.style.position = "";
                    div.style.display = "inline-block";
                    div.style.margin = "0px";
                    div.style.width = colwidth;
                    div.style.overflow = "hidden";
                    div.style.whiteSpace = "nowrap";
                    div.style.textOverflow = "clipped";

                    divCaption.style.display = "table";
                    divCaption.style.padding = "1px";
                    divCaption.style.width = "inherit"; //1.5in
                    divCaption.style.margin = "";
                    divCaption.style.cssFloat = "";
                    divCaption.textContent = caption;

                    divField.style.display = "table";
                    divField.style.padding = "1px";
                    divField.style.width = "inherit"; //1.5in
                    divField.style.margin = "";

                    div.setAttribute("data-fieldformat", "Block");
                    div.setAttribute("data-tabularcolwidth",colwidth);
                }
                else if (templateUpper == "FREEFORM" && reportType != enumReportTemplate.FreeForm) {
                    //div.style.position = "absolute";
                    //div.style.margin = "5px";
                    //div.style.left = left + "px";
                    //div.style.top = top + "px";
                    //div.setAttribute("data-fieldformat", "Inline");

                    div.style.position = "";
                    div.style.display = "inline-block";
                    div.style.margin = "0px";
                    div.style.border = "none";
                    div.style.width = "";

                    divCaption.style.margin = "5px";
                    divCaption.style.width = "auto";
                    divCaption.style.cssFloat = "";

                    divField.style.margin = "5px";
                    divField.width = "auto";
                    div.setAttribute("data-fieldformat", "Block");

                }
            }
            else if (div.id.startsWith("div") && templateUpper == "TABULAR" && reportType != enumReportTemplate.Tabular) {
                divDrop.removeChild(div);
            }
        }
        if (templateUpper == "TABULAR" && reportType != enumReportTemplate.Tabular) {
            reindexItems(divDrop);
            resetItemsSize(divDrop);
            setColWidthBorders();
        }
        else if (templateUpper == "FREEFORM" && reportType != enumReportTemplate.FreeForm) {
            reportView.ReportCaptionAlign = "Right";
            setReportTextAlign("ReportCaptionAlign","Right");
            reportView.ReportDetailAlign = "Left";
            setReportTextAlign("ReportDetailAlign","Left");
        }
    }
}

function setReportTextAlign(menuID,textalign) {
    var divDrop = document.getElementById("divDrop");
    var divItem;
    var div;

    if (menuID == "ReportDetailAlign") {
        for (var i = 0; i < divDrop.children.length; i++) {
            divItem = divDrop.children[i];
            if (divItem.id.startsWith("div_")) {
                div = divItem.children[1];
                div.dataset.textalign = textalign;
                if (textalign != "Auto")
                    div.style.textAlign = textalign.toLowerCase();
                else
                    div.style.textAlign = "";
            }
         }
    }
    else if (menuID == "ReportCaptionAlign") {
        for (var i = 0; i < divDrop.children.length; i++) {
            divItem = divDrop.children[i];
            if (divItem.id.startsWith("div_")) {
                div = divItem.children[0];
                div.dataset.textalign = textalign;
                div.style.textAlign = textalign.toLowerCase();
            }
        }
    }
}

function changeTextAlign(div,textalign) {

  if (div != void 0) {
    if (miID == "ReportDetailAlign") {
        reportView.ReportDetailAlign = textalign;
        setReportTextAlign(miID,textalign);
        showSection("divDrop");
    }
    else if (miID == "ReportCaptionAlign") {
        reportView.ReportCaptionAlign = textalign;
        setReportTextAlign(miID,textalign);
        showSection("divDrop");
    }
    else if (!div.id.startsWith("div_")) {
        if (div.dataset.textalign != void 0) {
           div.dataset.textalign = textalign;
           if (div.id.startsWith("divLabel_")) {
              var divCaption = div.children[0];
              divCaption.style.textAlign = textalign.toLowerCase();
           }
           else {
              div.style.textAlign = textalign.toLowerCase();
           }
        }
   }
   else {
            if (miID == "CaptionTextAlign") {
                div.children[0].dataset.textalign = textalign;
                div.children[0].style.textAlign = textalign.toLowerCase();
            }
            else if (miID == "DetailTextAlign") {
                div.children[1].dataset.textalign = textalign;
                div.children[1].style.textAlign = textalign.toLowerCase();
            }
        }
  }
}

function changeReportFieldLayout(layout) {
    var reportType = reportView.ReportTemplate;
    var  fieldFormat = reportView.ReportFieldLayout.toUpperCase();
    var layoutUpper = layout.toUpperCase();
    var div;

    if (reportType != enumReportTemplate.Tabular) {
        //if (fieldFormat  != layoutUpper) {
            for (var i = 0; i < divDrop.children.length; i++) {
                div = divDrop.children[i];
                if (div.id.startsWith("div_")) {
                    changeFieldLayout(div,layout);
                }
            }
            reportView.ReportFieldLayout = layout;
        //}
    }
}

function changeFieldLayout(div, layout) {
    var reportType = reportView.ReportTemplate;
    var layoutUpper = layout.toUpperCase();
    var fieldFormat ;
    
    if (div != void 0 && reportType != enumReportTemplate.Tabular) {
         fieldFormat = div.dataset.fieldformat.toUpperCase();
        
        //var divRect = div.getBoundingClientRect();
        var left = div.offsetLeft;
        var top = div.offsetTop;
        var divCaption = div.children[0];
        var divField = div.children[1];

        if (fieldFormat  != layoutUpper)  {
            div.style.width = "auto";
            div.style.height = "auto";
            div.style.left = left + "px";
            div.style.top = top + "px";

            if (layoutUpper == "INLINE") {

                div.style.border = "none";

                divCaption.style.display="inline-block";
                divCaption.style.margin = "0px 5px 0px 0px";
                divCaption.style.padding = "5px";
                divCaption.style.cssFloat = "left";

                divField.style.display="inline-block";
                divField.style.margin = "0px 0px 0px 0px ";
                divField.style.padding = "5px";


                div.setAttribute("data-fieldformat", "Inline");
            } else {

                div.style.margin = "0px";

                divCaption.style.display="";
                divField.style.display="";
                divCaption.style.margin = "5px";
                divField.style.margin = "5px";
                divCaption.style.padding = "5px";
                divField.style.padding = "5px";
                divCaption.style.cssFloat = "";
                div.setAttribute("data-fieldformat", "Block");
            }
            fieldFormat = div.dataset.fieldformat.toUpperCase();

            var rctCaption=divCaption.getBoundingClientRect();
            var rctField = divField.getBoundingClientRect();

            divCaption.setAttribute("data-dropwidth", parseInt(rctCaption.width,10));
            divCaption.setAttribute("data-dropheight", parseInt(rctCaption.height,10));
            divCaption.setAttribute("data-dropleft",left);
            divCaption.setAttribute("data-droptop", top);
            divCaption.style.left = left;
            divCaption.style.top = top;
            divCaption.style.width = divCaption.dataset.dropwidth;
            divCaption.style.height = divCaption.dataset.dropheight;

            divField.setAttribute("data-dropwidth", parseInt(rctField.width,10));
            divField.setAttribute("data-dropheight", parseInt(rctField.height,10));

            if (fieldFormat=="INLINE") {
                divField.setAttribute("data-dropleft",parseInt(left+rctCaption.width+3,10))
                divField.setAttribute("data-droptop", top);
            }
            else if (fieldFormat=="BLOCK") {
                divField.setAttribute("data-dropleft", left);
                divField.setAttribute("data-droptop",parseInt(top+rctCaption.height+3,10))
            }
            divField.style.left = divField.dataset.dropleft;
            divField.style.top = divField.dataset.droptop;
            divField.style.width = divField.dataset.dropwidth;
            divField.style.height = divField.dataset.dropheight;
        }
    }
    else {
        changeReportFieldLayout(layout);
    }
}

function fieldExists(liId) {
    if (liId == void 0 || liId == "")
        return false;

    var divDrop = document.getElementById("divDrop");
    var li = document.getElementById(liId).cloneNode(true);
    
    if (li != void 0) {
        var field = li.textContent;

        for (var i = 0; i < divDrop.children.length; i++) {
            var div = divDrop.children[i];
            if (div.id.startsWith("div_")) {
                var divField = div.children[1];
                
                if (divField != void 0 && divField.textContent == field) {
                    return true;
                }
            }
        }
    }
    return false;
}
function setColWidthBorders(divParent) {
    if (divParent != void 0 && reportView.ReportTemplate == enumReportTemplate.Tabular) {
        for (var i = 0; i < divParent.children.length; i++) {
            var div = divParent.children[i];
            if (div.id != "Line") {
                    div.style.borderRight = "solid 1px gray";
                    div.style.borderBottom = "solid 1px gray";

            }

        }
    }
}

function saveAndShow() {
    saveReportView();
    if (errorMessage == "") {
        var json = reportView.ToJSON();
        var data = JSON.stringify(json);
        showSpinner();
        __doPostBack("SaveReportView",data + "~" + "Show Report");
    }
    else
     errorMessage = "";
}

function saveAndReturn () {
    saveReportView();
    if (errorMessage == "") {
        var json = reportView.ToJSON();
        var data = JSON.stringify(json);
        showSpinner();
        __doPostBack("SaveReportView",data + "~" + "Return To Designer");
    }
    else
        errorMessage = "";

}

function saveAndClose() {
    saveReportView();
    if (errorMessage == "") {
        var json = reportView.ToJSON();
        var data = JSON.stringify(json);
        showSpinner();
        __doPostBack("SaveReportView",data + "~" + "Close Designer");
    }
    else
        errorMessage = "";
}

function saveReportView() {
    var hdnReportView = document.getElementById("hdnReportView");
    var hdnDataBase = document.getElementById("hdnDataBase");
    var divDrop = document.getElementById("divDrop");
    var divHeader = document.getElementById("divHeader");
    var divFooter = document.getElementById("divFooter");
    var divGroup = document.getElementById("divGroup");
    var offsetLeft = divDrop.offsetLeft;
    var offsetTop = divDrop.offsetTop;
    var dropRect = divDrop.getBoundingClientRect();
    var reportType = reportView.ReportTemplate;
    var hasGroups = false;

    if (divGroup != void 0 && divGroup.children.length > 0)
        hasGroups = true;



    var itm;
    var parts;
    var id;
    var divCaption;
    var divField;
    var caption;
    var tblfld;
    var divRect;
    var order;
    var itemtype;
    var nChildren = divDrop.children.length-1; // divDrop always has one child.

    errorMessage="";
    if (nChildren == 0) {
        errorMessage = "There are no detail fields defined. Report data can't be saved."
        showMessage(errorMessage,"No Detail Fields Defined",enumMessageType.Error);
        return;
    }
    reportView.Items.Clear();

    for (var i = 0; i < divDrop.children.length; i++) {
        var div = divDrop.children[i];
        var tabularColWidth = div.dataset.tabularcolwidth;

        itm = null;

        if (div.id.startsWith("div_")) {
            parts = div.id.split("div_");
            id = parts[1];
            divCaption = div.children[0];
            divField = div.children[1];
            caption = divCaption.textContent;
            tblfld = div.dataset.fieldname; //div.getAttribute("data-fieldname");
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;

            parts = tblfld.split(".");

            var tbl = parts[0];
            var fld = parts[1];
            var datatype = div.dataset.fieldtype;
            var fldLayout = div.dataset.fieldformat;
            var database = hdnDataBase.value;

            itm = new ReportItem;
            getFontSettings(divCaption,itm);
            getFontSettings(divField,itm);
            getDisplaySettings(divCaption,itm);
            getDisplaySettings(divField,itm);

            itm.Caption = caption;
            itm.CaptionHeight = divCaption.dataset.dropheight;
            itm.CaptionWidth = divCaption.dataset.dropwidth;
            itm.CaptionX = divCaption.dataset.dropleft;
            itm.CaptionY = divCaption.dataset.droptop;
            itm.CaptionTextAlign = divCaption.dataset.textalign;
            itm.SQLDatabase = database;
            itm.SQLTable = tbl;
            itm.SQLField = fld;
            itm.SQLDataType = datatype;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            if (fldLayout == "Block")
                itm.FieldLayout = enumFieldLayout.Block;
            else
                itm.FieldLayout = enumFieldLayout.Inline;
            itm.DataHeight = divField.dataset.dropheight;
            itm.DataWidth = divField.dataset.dropwidth;
            itm.DataX = divField.dataset.dropleft;
            itm.DataY = divField.dataset.droptop;
            itm.TextAlign = divField.dataset.textalign;
            itm.Width = div.dataset.dropwidth; //parseInt(divRect.width, 10);
            itm.Height = div.dataset.dropheight; //parseInt(divRect.height, 10);
            itm.X = div.dataset.dropleft;
            itm.Y = div.dataset.droptop;
            if (reportType == enumReportTemplate.Tabular) {
                itm.TabularColumnWidth = (tabularColWidth != void 0 && tabularColWidth != "") ? tabularColWidth:"1.5in";
            }

            itm.Section="Details";
        }
        else if (div.id.startsWith("divLabel_")) {
            parts = div.id.split("divLabel_");
            id = "Label_" + parts[1];
            divCaption = div.children[0];
            caption = divCaption.textContent;
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;

            itm = new ReportItem;
            itm.ReportItemType = enumItemType.Label;
            getFontSettings(div,itm);
            getDisplaySettings(div,itm);
            itm.Caption = caption;
            itm.FieldLayout = enumFieldLayout.None;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            itm.ReportItemType = enumItemType.Label;
            itm.TextAlign = div.dataset.textalign;
            itm.Width = div.dataset.dropwidth; //parseInt(divRect.width, 10);
            itm.Height = div.dataset.dropheight; //parseInt(divRect.height, 10);
            itm.X = div.dataset.dropleft;//parseInt(div.style.left,10); //parseInt(divRect.left - offsetLeft, 10);
            itm.Y = div.dataset.droptop; //parseInt(div.style.top,10);//parseInt(divRect.top - offsetTop, 10);
            itm.Section = "Details"
        }

        if (itm != null)
            reportView.Items.Add(itm);

    }
    for (var i =0; i < divHeader.children.length; i++) {
        var div = divHeader.children[i];
        itm = null;
        if (div.id.startsWith("divLabel_")) {
            parts = div.id.split("divLabel_");
            id = "Label_" + parts[1];
            divCaption = div.children[0];
            caption = divCaption.textContent;
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;

            itm = new ReportItem;
            itm.ReportItemType = enumItemType.Label;
            getFontSettings(div,itm);
            getDisplaySettings(div,itm);
            itm.Caption = caption;
            itm.FieldLayout = enumFieldLayout.None;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            itm.TextAlign = div.dataset.textalign;
            itm.Width = div.dataset.dropwidth; //parseInt(divRect.width, 10);
            itm.Height = div.dataset.dropheight; // parseInt(divRect.height, 10);
            itm.X = div.dataset.dropleft; //parseInt(div.style.left,10);
            itm.Y = div.dataset.droptop; //parseInt(div.style.top,10);
            itm.Section = "Header";
        }
        else if (div.id.startsWith("div")) {
            itemtype = div.dataset.itemtype;
            parts = div.id.split("_");
            id = itemtype + "_" + parts[1];
            caption = div.textContent;
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;

            itm = new ReportItem;
            itm.ReportItemType = getReportItemTypeEnum(itemtype);
            getFontSettings(div,itm);
            getDisplaySettings(div,itm);
            itm.Caption = caption;
            itm.FieldLayout = enumFieldLayout.None;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            itm.TextAlign = div.dataset.textalign;
            if (itemtype.toLowerCase() == "image") {
                itm.ImageWidth = div.dataset.imagewidth;
                itm.ImageHeight = div.dataset.imageheight;
                itm.ImagePath = div.dataset.imagepath;
                itm.ImageData = div.dataset.imagedata;
                itm.Caption = div.dataset.imagecaption;
            }
            itm.Width = div.dataset.dropwidth; //parseInt(divRect.width, 10);
            itm.Height = div.dataset.dropheight; //parseInt(divRect.height, 10);
            itm.X = div.dataset.dropleft; //parseInt(div.style.left,10); 
            itm.Y = div.dataset.droptop; //parseInt(div.style.top,10);
            itm.Section = "Header";
        }
        if (itm != null)
            reportView.Items.Add(itm);
    }
    for (var i =0; i < divFooter.children.length; i++) {
        var div = divFooter.children[i];
        itm = null;
        if (div.id.startsWith("divLabel_")) {
            parts = div.id.split("divLabel_");
            id = "Label_" + parts[1];
            divCaption = div.children[0];
            caption = divCaption.textContent;
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;

            itm = new ReportItem;
            itm.ReportItemType = enumItemType.Label;
            getFontSettings(div,itm);
            getDisplaySettings(div,itm);
            itm.Caption = caption;
            itm.FieldLayout = enumFieldLayout.None;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            itm.TextAlign = div.dataset.textalign;
            itm.Width = div.dataset.dropwidth; //parseInt(divRect.width, 10);
            itm.Height = div.dataset.dropheight;// parseInt(divRect.height, 10);
            itm.X = div.dataset.dropleft; //parseInt(div.style.left,10); 
            itm.Y = div.dataset.droptop //parseInt(div.style.top,10);
            itm.Section = "Footer";
        }
        else if (div.id.startsWith("div")) {
            itemtype = div.dataset.itemtype;
            parts = div.id.split("_");
            id = itemtype + "_" + parts[1];
            caption = div.textContent;
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;

            itm = new ReportItem;
            itm.ReportItemType = getReportItemTypeEnum(itemtype);
            getFontSettings(div,itm);
            getDisplaySettings(div,itm);
            itm.Caption = caption;
            itm.FieldLayout = enumFieldLayout.None;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            itm.TextAlign = div.dataset.textalign;
            if (itemtype.toLowerCase() == "image") {
                itm.ImageWidth = div.dataset.imagewidth;
                itm.ImageHeight = div.dataset.imageheight;
                itm.ImagePath = div.dataset.imagepath;
                itm.ImageData = div.dataset.imagedata;
                itm.Caption = div.dataset.imagecaption;
            }
            itm.Width = div.dataset.dropwidth; //parseInt(divRect.width, 10);
            itm.Height = div.dataset.dropheight; //parseInt(divRect.height, 10);
            itm.X = div.dataset.dropleft; //parseInt(div.style.left,10); 
            itm.Y = div.dataset.droptop; //parseInt(div.style.top,10);
            itm.Section = "Footer";
        }
        if (itm != null)
            reportView.Items.Add(itm);
    }

    if (hasGroups) {
        for (var i = 0; i < divGroup.children.length; i++) {
            var div = divGroup.children[i];
            itm = null;

            parts = div.id.split("_");
            id = parts[1] + "_" + parts[2];
            caption = div.textContent;
            divRect = div.getBoundingClientRect();
            order = div.dataset.fieldindex;
            itemtype = div.dataset.itemtype;

            itm = new ReportItem;
            itm.ReportItemType = getReportItemTypeEnum(itemtype);
          
            getFontSettings(div, itm);
            getDisplaySettings(div, itm);
            itm.Caption= caption;
            itm.FieldLayout = enumFieldLayout.None;
            itm.ItemID = id;
            itm.ItemOrder = parseInt(order, 10);
            itm.TextAlign = div.dataset.textalign;
            itm.Height = parseInt(div.style.height, 10);
            itm.X = parseInt(div.style.left, 10);
            itm.Y = parseInt(div.style.top, 10);
            itm.Section = "Groups";

            if (itm != null)
                reportView.Items.Add(itm);
        }
    }
    //var json = reportView.ToJSON();
    //var data = JSON.stringify(json);
    //__doPostBack("SaveReportView",data);
    //hdnReportView.value = JSON.stringify(json);
    //return true;
}
function getReportItemTypeEnum(reportItemTypeStr) {
    var itemtype = enumItemType.DataField;
    switch (reportItemTypeStr) {
        case "DataField":
            itemtype = enumItemType.DataField;
            break;
        case "FormulaField":
            itemtype = enumItemType.FormulaField;
            break;
        case "Section":
            itemtype = enumItemType.Section;
            break;
        case "Label":
            itemtype = enumItemType.Label;
            break;
        case "PageNumber":
            itemtype = enumItemType.PageNumber;
            break;
        case "PageTotal":
            itemtype = enumItemType.PageTotal;
            break;
        case "PageNofM":
            itemtype = enumItemType.PageNofM;
            break;
        case "ReportTitle":
            itemtype = enumItemType.ReportTitle;
            break;
        case "ReportUser":
            itemtype = enumItemType.ReportUser;
            break;
        case "ReportUserFML":
            itemtype = enumItemType.ReportUserFML;
            break;
        case "ReportUserLF":
            itemtype = enumItemType.ReportUserLF;
            break;
        case "ReportUserLFM":
            itemtype = enumItemType.ReportUserLFM;
            break;
        case "ReportUserF":
            itemtype = enumItemType.ReportUserF;
            break;
        case "ReportUserM":
            itemtype = enumItemType.ReportUserM;
            break;
        case "ReportUserL":
            itemtype = enumItemType.ReportUserL;
            break;
        case "PrintDate":
            itemtype = enumItemType.PrintDate;
            break;
        case "PrintTime":
            itemtype = enumItemType.PrintTime;
            break;
        case "PrintDateTime":
            itemtype = enumItemType.PrintDateTime;
            break;
        case "SqlQuery":
            itemtype = enumItemType.Query;
            break;
        case "ConfidentialityStatement":
            itemtype = enumItemType.ConfidentialityStatement;
            break;
        case "ReportComments":
            itemtype=enumItemType.ReportComments;
            break;
        case "Image":
            itemtype = enumItemType.Image;
            break;
        case "Group":
            itemtype = enumItemType.Group;
            break;
        default:
            itemtype = enumItemType.DataField;

    }
    return itemtype;
}
function setDisplaySettings(div) {
    var ds = new DisplaySettings;
    if (div != void 0) {
        var backColor = div.dataset.backcolor || "white"; 
        var borderColor = div.dataset.bordercolor || "lightgrey";
        var borderStyle = div.dataset.borderstyle || "Solid";
        var borderWidth = div.dataset.borderwidth || "1";

        ds.BackColorName = backColor;
        ds.BorderColorName = borderColor;
        ds.BorderStyle = borderStyle;
        ds.BorderWidth = borderWidth;

        if (backColor.startsWith("#"))
            ds.BackColorHex = backColor;
        else
            ds.BackColorHex = toHex(backColor);

        if (borderColor.startsWith("#"))
            ds.BorderColorHex = borderColor;
        else
            ds.BorderColorHex = toHex(borderColor);
    }
    else if (div.id.startsWith("div"))
        ds=dsLabel;
    else if (div.id.startsWith("caption_"))
        ds = dsCaption;
    else
        ds = dsDetail;

    return ds;
}
function setFontSettings(div) {
    var fs = new FontSettings;
    if (div != void 0) {
        var fontName = div.style.fontFamily || "Tahoma";
        var fontStyle = div.style.fontStyle || "normal";
        var fontWeight = div.style.fontWeight || "normal";
        var fontSize = div.style.fontSize || "12px";
        var textDecoration = div.style.textDecoration || "none";
        var clr = div.style.color || "black";

        fontName = fontName.replaceAll('"','');

        if (div.id.startsWith("div")) {
            fs.FontName = fontName;

            if (clr.startsWith("rgb(")) {
                fs.ColorHex = toHexFromRGB(clr);
                fs.ColorName = fs.ColorHex;
            }
            else {
                fs.ColorName = clr;
                fs.ColorHex = toHex(clr);
            }

            if (fontStyle == "normal" && fontWeight == "normal")
                fs.FontStyle = "Regular";
            else if (fontStyle == "italic" && fontWeight == "normal")
                fs.FontStyle = "Italic";
            else if (fontStyle == "normal" && fontWeight == "bold")
                fs.FontStyle = "Bold";
            else if (fontStyle == "italic" && fontWeight == "bold")
                fs.FontStyle = "Bold Italic"

            if (textDecoration == "underline line-through") {
                fs.Underline = true;
                fs.Strikeout = true;
            }
            if (textDecoration == "underline") 
                fs.Underline = true;
            else if (textDecoration == "line-through")
                fs.Strikeout = true;

            fs.FontSize = fontSize;
        }
        else if (div.id.startsWith("caption_")) {
            fs.FontName = reportView.LabelFontName;
            fs.FontSize = reportView.LabelFontSize;
            fs.ColorName = reportView.LabelForeColor;
            fs.FontStyle = reportView.LabelFontStyle;
            fs.Underline = reportView.LabelUnderline;
            fs.Strikeout = reportView.LabelStrikeout;
        }
        else if (div.id.startsWith("field_")) {
            fs.FontName = reportView.DataFontName;
            fs.FontSize = reportView.DataFontSize;
            fs.ColorName = reportView.DataForeColor;
            fs.FontStyle = reportView.DataFontStyle;
            fs.Underline = reportView.DataUnderline;
            fs.Strikeout = reportView.DataStrikeout;
        }
    }
    else
        fs=fsLabel;
    return fs;
}
function getDisplaySettings(div,itm) {
    if (div != void 0 && itm != void 0) {
        var backColor = div.dataset.backcolor || "white";
        var borderColor = div.dataset.bordercolor || "lightgrey";
        var borderWidth = div.dataset.borderwidth || "1";
        var borderStyle = div.dataset.borderstyle || "Solid";

        itm.CaptionBackColor = backColor;
        itm.CaptionBorderColor = borderColor;
        itm.CaptionBorderStyle = borderStyle;
        itm.CaptionBorderWidth = borderWidth;

        if (div.id.startsWith("caption_") || div.id.startsWith("divLabel_")) {
            itm.CaptionBackColor = backColor;
            itm.CaptionBorderColor = borderColor;
            itm.CaptionBorderStyle = borderStyle;
            itm.CaptionBorderWidth = borderWidth;
        }
        else if (div.id.startsWith("field_") || div.id.startsWith("div")) {
            itm.BackColor = backColor;
            itm.BorderColor = borderColor;
            itm.BorderStyle = borderStyle;
            itm.BorderWidth = borderWidth;
        }
    }
}
function getFontSettings(div,itm) {
    if (div != void 0 && itm != void 0) {
        var fontName = div.style.fontFamily || "Tahoma";
        var fontStyle = div.style.fontStyle || "normal";
        var fontWeight = div.style.fontWeight || "normal";
        var fontSize = div.style.fontSize || "12px";
        var textDecoration = div.style.textDecoration || "none";
        var clr = div.style.color || "black";
        var fs = new FontSettings();

        fontName = fontName.replaceAll('"','');
        
        if (div.id.startsWith("div")) {
            fs.FontName = fontName;

            if (clr.startsWith("rgb(")) {
                fs.ColorHex = toHexFromRGB(clr);
                fs.ColorName = fs.ColorHex;
            }
            else {
                fs.ColorName = clr;
                fs.ColorHex = toHex(clr);
            }

            if (fontStyle == "normal" && fontWeight == "normal")
                fs.FontStyle = "Regular";
            else if (fontStyle == "italic" && fontWeight == "normal")
                fs.FontStyle = "Italic";
            else if (fontStyle == "normal" && fontWeight == "bold")
                fs.FontStyle = "Bold";
            else if (fontStyle == "italic" && fontWeight == "bold")
                fs.FontStyle = "Bold Italic"

            if (textDecoration == "underline line-through") {
                fs.Underline = true;
                fs.Strikeout = true;
            }
            if (textDecoration == "underline") 
                fs.Underline = true;
            else if (textDecoration == "line-through")
                fs.Strikeout = true;

            fs.FontSize = fontSize;
        }
        else if (div.id.startsWith("caption_")) {
            fs.FontName = reportView.LabelFontName;
            fs.FontSize = reportView.LabelFontSize;
            fs.ColorName = reportView.LabelForeColor;
            fs.FontStyle = reportView.LabelFontStyle;
            fs.Underline = reportView.LabelUnderline;
            fs.Strikeout = reportView.LabelStrikeout;
        }
        else if (div.id.startsWith("field_")) {
            fs.FontName = reportView.DataFontName;
            fs.FontSize = reportView.DataFontSize;
            fs.ColorName = reportView.DataForeColor;
            fs.FontStyle = reportView.DataFontStyle;
            fs.Underline = reportView.DataUnderline;
            fs.Strikeout = reportView.DataStrikeout;
        }

        if (div.id.startsWith("caption_") || div.id.startsWith("divLabel_")) {
            itm.CaptionFontName = fs.FontName;
            itm.CaptionFontSize = fs.FontSize;
            itm.CaptionForeColor = fs.ColorName;
            itm.CaptionFontStyle = fs.FontStyle;
            itm.CaptionUnderline = fs.Underline;
            itm.CaptionStrikeout = fs.Strikeout;
        }
        else if (div.id.startsWith("field_") || div.id.startsWith("div")) {
            itm.FontName = fs.FontName;
            itm.FontSize = fs.FontSize;
            itm.ForeColor = fs.ColorName;
            itm.FontStyle = fs.FontStyle;
            itm.Underline = fs.Underline;
            itm.Strikeout = fs.Strikeout;
        }
    }
}

function doImageSettings(div, ds) {
    if (div != void 0) {
        var img = div.children[0];


        if (ds.ImageWidth != "" && ds.ImageHeight != "") {
            var width = (parseInt(ds.ImageWidth, 10) + 10)
            var height = (parseInt(ds.ImageHeight, 10) + 10)
            var aspectRatio;
            div.style.width = width + "px";
            div.style.height = height + "px";

            div.setAttribute("data-imagewidth", ds.ImageWidth);
            div.setAttribute("data-imageheight", ds.ImageHeight);
            div.setAttribute("data-sizeoption", ds.SizeOption);
            //if (ds.ImageWidth != ds.ImageHeight) {
            //    div.setAttribute("data-sizeoption", "KeepAspectRatio");
            //    aspectRatio = parseFloat(parseFloat(ds.ImageWidth).toFixed() / parseFloat(ds.ImageHeight).toFixed()).toFixed(2);
            //    div.setAttribute("data-aspectratio", aspectRatio.toString());
            //}
            div.setAttribute("data-dropwidth", width);
            div.setAttribute("data-dropheight", height);
        }
        div.style.borderColor = ds.BorderColorName;
        div.style.borderWidth = ds.BorderWidth + "px";
        div.style.borderStyle = ds.BorderStyle.toLowerCase();

        div.setAttribute("data-bordercolor", ds.BorderColorName);
        div.setAttribute("data-borderstyle", ds.BorderStyle);
        div.setAttribute("data-borderwidth", parseInt(ds.BorderWidth, 10));
        div.setAttribute("data-imagepath", ds.ImagePath);
        div.setAttribute("data-imagedata", ds.ImageData);


        if (img != void 0 && img != "undefined") {
            if (ds.ImageData != "" && ds.ImageData.startsWith("data:image/")) {

                img.addEventListener("load", getAspectRatioOnImageLoad);
                img.src = ds.ImageData;
            }
            else if (ds.ImagePath != "" && ds.ImagePath.includes("/")) {
                img.addEventListener("load", getAspectRatioOnImageLoad);
               img.src = ds.ImagePath
              }

        }
       
    }
}
function doDisplaySettings(div,ds) {
    if (div != void 0) {
        div.style.backgroundColor = ds.BackColorName;
        div.style.borderColor = ds.BorderColorName;
        div.style.borderStyle = ds.BorderStyle.toLowerCase();
        div.style.borderWidth = ds.BorderWidth + "px";

        div.setAttribute("data-backcolor",ds.BackColorName);
        div.setAttribute("data-bordercolor",ds.BorderColorName);
        div.setAttribute("data-borderstyle",ds.BorderStyle);
        div.setAttribute("data-borderwidth",parseInt(ds.BorderWidth,10))
    }
}
function doSettings(div,fs) {
    if (div != void 0) {
        div.style.fontFamily = fs.FontName;
        if (fs.FontStyle.toLowerCase() == "regular") {
            div.style.fontStyle = "normal";
            div.style.fontWeight = "normal";
        }
        else if (fs.FontStyle.toLowerCase() == "italic") {
            div.style.fontStyle = "italic";
            div.style.fontWeight = "normal";
        }
        else if (fs.FontStyle.toLowerCase() == "bold" ) {
            div.style.fontStyle = "normal";
            div.style.fontWeight = "bold";

        }
        else if (fs.FontStyle.toLowerCase() == "bold italic") {
            div.style.fontStyle = "italic";
            div.style.fontWeight= "bold";
        }
        div.style.fontSize=fs.FontSize;

        if (fs.Strikeout && fs.Underline)
            div.style.textDecoration = "underline line-through";
        else if (fs.Strikeout) 
            div.style.textDecoration = "line-through";
        else if (fs.Underline)
            div.style.textDecoration = "underline";
        else
            div.style.textDecoration = "none";

        div.style.color=fs.ColorName;

    }
}
function createDivFromItem(item) {
    var id = item.ItemID
    var div = document.createElement("div");
    var reportType = reportView.ReportTemplate;
    var fieldType = item.SQLDataType;
    var caption = item.Caption;
    var itm = reportView.Items.GetItem(id);
    var tabularColWidth = (itm.TabularColumnWidth!=void 0 && itm.TabularColumnWidth != "")?itm.TabularColumnWidth:"1.5in";
    var fs = new FontSettings();
    var tblfld = item.SQLTable + "." + item.SQLField;
    var textAlign = item.TextAlign;
    var captionTextAlign = item.CaptionTextAlign;

    var captionBackColor = dsCaption.BackColorName;
    var captionBorderColor = dsCaption.BorderColorName;
    var captionBorderStyle = dsCaption.BorderStyle;
    var captionBorderWidth = dsCaption.BorderWidth;
    var valueBackColor = dsDetail.BackColorName;
    var valueBorderColor = dsDetail.BorderColorName;
    var valueBorderStyle = dsDetail.BorderStyle;
    var valueBorderWidth = dsDetail.BorderWidth;

    if (reportType == enumReportTemplate.FreeForm) {
        captionBackColor = item.CaptionBackColor;
        captionBorderColor = item.CaptionBorderColor;
        captionBorderStyle = item.CaptionBorderStyle;
        captionBorderWidth = item.CaptionBorderWidth;
        valueBackColor = item.BackColor;
        valueBorderColor = item.BorderColor;
        valueBorderStyle = item.BorderStyle;
        valueBorderWidth = item.BorderWidth;
    }

    div.id = "div_" + id;
    div.setAttribute("draggable", "true");
    div.setAttribute("ondragstart", "dragDiv(event);");
    div.setAttribute("ondrop", "dropOnDiv(event);");
    div.setAttribute("oncontextmenu", "return showMenu(event);")
    div.setAttribute("data-fieldname", tblfld);
    div.setAttribute("data-fieldtype", fieldType);
    div.setAttribute("tabindex","-1")
    div.classList.add("divitem");

    if (reportType == enumReportTemplate.Tabular) {
        div.style.width = tabularColWidth;
        div.setAttribute("data-tabularcolwidth",tabularColWidth);
        div.style.position="relative";
        div.style.overflow = "hidden";
        div.style.whiteSpace = "nowrap";
        div.style.textOverflow = "clipped";
    }
    else {
        div.style.width = "auto";
    }

        
    div.style.display = "inline-block";

    var divCaption = document.createElement("div");
    var title = tblfld;

    fs=fsCaption;
    doSettings(divCaption,fs);
    if (caption != void 0) {
        var parts = caption.split(":");
        title = parts[0];
    }
    divCaption.title = title;
    divCaption.id = "caption_" + id;
    divCaption.style.display = "table";

    if (reportType == enumReportTemplate.FreeForm) {
        divCaption.style.display = "";
        divCaption.style.margin = (item.FieldLayout == enumFieldLayout.Inline) ? "0px 5px 0px 0px":"5px";
        divCaption.style.padding = "5px";

        divCaption.style.overflow = "hidden";
        divCaption.style.whiteSpace = "nowrap";
        divCaption.style.textOverflow = "clipped";

        divCaption.style.width =  item.CaptionWidth + "px"; //"auto";
        divCaption.style.height = item.CaptionHeight + "px"; //"auto";
        divCaption.style.cssFloat = (item.FieldLayout == enumFieldLayout.Inline) ? "left":"";
        if (item.FieldLayout == enumFieldLayout.Inline) {
            divCaption.style.display = "inline-block";
         }
    }
    else {
        divCaption.style.padding = "1px";
        divCaption.style.width = "inherit";
        divCaption.style.height = "auto";
    }
    divCaption.style.backgroundColor = captionBackColor;
    divCaption.style.borderColor = captionBorderColor;
    divCaption.style.borderStyle = captionBorderStyle.toLowerCase();
    divCaption.style.borderWidth = captionBorderWidth + "px";
    divCaption.style.textAlign = captionTextAlign.toLowerCase();

    if (reportType == enumReportTemplate.Tabular) {
        divCaption.textContent = title;
    }
    else {
        if (itm == void 0 || itm.FieldLayout == enumFieldLayout.Inline)
            divCaption.textContent = title + ": ";
        else if (itm.FieldLayout == enumFieldLayout.Block)
            divCaption.textContent = title;
    }
    divCaption.setAttribute("data-textalign",captionTextAlign);
    divCaption.setAttribute("data-backcolor",captionBackColor);
    divCaption.setAttribute("data-bordercolor",captionBorderColor);
    divCaption.setAttribute("data-borderstyle",captionBorderStyle);
    divCaption.setAttribute("data-borderwidth",parseInt(captionBorderWidth,10));
    divCaption.addEventListener("click", onCaptionClick);
    div.appendChild(divCaption);

    var divField = document.createElement("div");

    fs = fsDetail;
    doSettings(divField,fs);

    divField.title = tblfld;
    divField.id = "field_" + id;
    divField.style.display = "table";

    if (reportType == enumReportTemplate.FreeForm) {
        divField.style.display = "";
        divField.style.margin = (item.FieldLayout == enumFieldLayout.Inline) ? "0px 0px 0px 0px":"5px";
        divField.style.padding = "5px";

        divField.style.overflow = "hidden";
        divField.style.whiteSpace = "nowrap";
        divField.style.textOverflow = "clipped";

        divField.style.width = item.DataWidth + "px";
        divField.style.height = item.DataHeight + "px";
        if (item.FieldLayout == enumFieldLayout.Inline) {
            divField.style.display = "inline-block";
         }
    }
    else {
        divField.style.padding = "1px";
        divField.style.width = "inherit";
        divField.style.height = "auto";
    }
    divField.style.backgroundColor = valueBackColor; //"lightyellow";
    divField.style.borderColor = valueBorderColor;
    divField.style.borderStyle = valueBorderStyle.toLowerCase();
    divField.style.borderWidth = valueBorderWidth + "px";

    if (textAlign != "Auto")
      divField.style.textAlign = textAlign.toLowerCase();
    divField.textContent = tblfld;

    divField.setAttribute("data-textalign",textAlign);
    divField.setAttribute("data-backcolor",valueBackColor);
    divField.setAttribute("data-bordercolor",valueBorderColor);
    divField.setAttribute("data-borderstyle",valueBorderStyle);
    divField.setAttribute("data-borderwidth",parseInt(valueBorderWidth,10));

    div.appendChild(divField);

    return div;
}
function findReportColumn(columnID) {
    var divDrop = document.getElementById("divDrop");
    var div = null;

    for (var i = 0; i < divDrop.children.length; i++) {
        div = divDrop.children[i];
        if (div.id == columnID)
            return div;
    }
    return div;
}
function applyColSizerChanges() {
    
    var divColWidthBody = document.getElementById("divTabularWidthBody");
    var divReportColumn;
    var div;
    var bChanged = false;
    for (var i = 0; i < divColWidthBody.children.length; i++) {
        div = divColWidthBody.children[i];
        if (div.dataset.widthchanged == "true") {
            var columnID = div.dataset.columnid;
            divReportColumn = findReportColumn(columnID);
            if (divReportColumn) {
                divReportColumn.dataset.tabularcolwidth=div.dataset.tabularcolwidth;
                divReportColumn.dataset.dropwidth = div.dataset.dropwidth;
                divReportColumn.style.width = div.style.width;
                bChanged = true;
            }
        }
    }
        closePopup('TabularWidthBackground');
        if (!bChanged)
            showMessage("No Column Widths Have Been Changed","No Changes Made",enumMessageType.Warning);
       else
         showSection("divDrop");
}

function hideTip() {
    var tip = document.getElementById("divTip");

    tip.style.display = "none";
    tip.style.left = "";
    tip.style.top = "";
    tip.textContent = "";

}

function hideSizeTip() {
    var tip = document.getElementById("divSizeTip");

    tip.style.display = "none";
    tip.style.left = "";
    tip.style.top = "";
    tip.textContent = "";
   
}

function showSizeTip(e,text) {
    var col = e.target;
    if (col) {
        var container=col.parentElement;
        var tip = document.getElementById("divSizeTip");
        var rctContainer = container.getBoundingClientRect();
        var rctCol = col.getBoundingClientRect();
        var rctTip = tip.getBoundingClientRect();
        var left = e.clientX -  rctContainer.x + container.offsetLeft + 5;
        var top = e.clientY - rctContainer.y + container.offsetTop +10;
        var sizer = col.children[0];

        if (sizer.style.backgroundColor!="blue") {
            tip.style.display="inline-block";
            tip.style.left = left + "px";
            tip.style.top = top + "px";
            tip.textContent = text;
        }
    }
}

function showTip(col,text) {
    if (col) {
        var container=col.parentElement;
        var tip = document.getElementById("divTip");
        var rctContainer = container.getBoundingClientRect();
        var rctCol = col.getBoundingClientRect();
        var rctTip = tip.getBoundingClientRect();
        var left = parseFloat(rctCol.right-rctContainer.left-(rctTip.width/2)).toFixed();
        var top = parseFloat(rctCol.top-rctContainer.top).toFixed() + 2;

        if (rctTip.width ==0) {
            var tipStyles = window.getComputedStyle(tip);
            var styleLeft =  parseInt(tipStyles.width,10);
            styleLeft = parseFloat(styleLeft/2).toFixed();
            
            left = left-styleLeft;
        }
        tip.style.display="inline-block";
        tip.style.left = left + "px";
        tip.style.top = top + "px";

        tip.textContent = text;
    }
}
//function onColSizerMouseOut(e) {
//    var resizer = e.target;
//    resizer.style.backgroundColor = "white";
//    hideTip();
//}

//function onColSizerMouseOver(e) {
//    var resizer = e.target;
//    var col = resizer.parentElement;
//    var tabularcolwidth = col.dataset.tabularcolwidth;

//    resizer.style.backgroundColor = "blue";
//    showTip(col,tabularcolwidth);
//}
function onColMouseOut(e) {
    var col= e.target;
    hideSizeTip();
      //hideTip();
}

function onColMouseOver(e) {
    
    var col = e.target
    var tabularcolwidth = col.dataset.tabularcolwidth;

   
    //showTip(col,tabularcolwidth);
    showSizeTip(e,tabularcolwidth);
}

function onColSizerMouseDown(e) {
    var resizer = e.target;
    curCol = resizer.parentElement;

    nxtCol = curCol.nextElementSibling;

    if (curCol) {
        hideSizeTip();
        //var container=curCol.parentElement;
        //var tip = document.getElementById("divTip");
        //var containRect = container.getBoundingClientRect();
        //var colRect = curCol.getBoundingClientRect();

        //tip.style.display="";
        //tip.style.left = (colRect.right-containRect.left) + "px";
        //tip.style.top = (colRect.top-containRect.top) + "px";
        pageX = e.pageX;
        //resizer.style.cursor = "ew-resize";
        styles = window.getComputedStyle(curCol);
        //curColWidth = parseInt(styles.width, 10);
        //var inches = curColWidth/96;
        var tabularcolwidth = curCol.dataset.tabularcolwidth;
        curColWidth = (parseFloat(tabularcolwidth).toFixed(2)*96); 
        showTip(curCol,tabularcolwidth);

        if (nxtCol) {
            styles = window.getComputedStyle(nxtCol); 
            nxtColWidth = parseInt(styles.width, 10);
        }
        document.addEventListener('mouseup',onColSizerMouseUp);
        document.addEventListener('mousemove',onColSizerMouseMove);
        resizer.classList.add("resizing");
        document.body.style.cursor = "ew-resize";
    }
}
function onColSizerMouseUp(e) {
    var resizer;
    var parent = e.target.parentElement;

    if (parent != void 0) {
        if (curCol) {
            resizer = curCol.children[0];
            var width = parseInt(curCol.style.width,10);
            var inches = width/96;
            var tabularcolwidth = inches.toFixed(2) + "in";
            curCol.dataset.tabularcolwidth=tabularcolwidth;
            curCol.dataset.widthchanged="true";
            curCol.setAttribute("data-dropwidth",width)
            //curCol.title=tabularcolwidth;
            resizer.classList.remove('resizing');
            hideTip();
            document.body.style.cursor = "default";
        }
        document.removeEventListener('mousemove', onColSizerMouseMove);
        document.removeEventListener('mouseup', onColSizerMouseUp);
        curCol = undefined;
        nxtCol = undefined;
        pageX = undefined;
        nxtColWidth = undefined;
        curColWidth = undefined
    }
}
function onColSizerMouseMove(e) {
    if (curCol) {
        var diffX = parseInt(e.pageX - pageX,10);

        var colWidth = curColWidth+diffX;
        curCol.style.width = colWidth + "px"; //parseInt(curColWidth + (diffX),10) + 'px';
        var inches = colWidth/96; //parseInt(curCol.style.width,10)/96;
        var tabularcolwidth = inches.toFixed(2) + "in";
        showTip(curCol,tabularcolwidth);
    }
}
function createResizer() {
    var div = document.createElement("div");
    div.classList.add("resizer");
    div.addEventListener('mousedown',onColSizerMouseDown);
    return div;
}

function createSizableColDiv(divChild) {
    var reportType = reportView.ReportTemplate;

    if (reportType == enumReportTemplate.Tabular && divChild != void 0 && divChild.id.startsWith("div_")) {
        var reportCaptionAlign = reportView.ReportCaptionAlign;
        var reportDetailAlign = reportView.ReportDetailAlign;
        var div = document.createElement("div");
        var divChildCaption = divChild.children[0];
        var divChildField = divChild.children[1];
        var ids = divChild.id.split("_");
        var captionids = divChildCaption.id.split("_");
        var fieldids = divChildField.id.split("_");
        var captionID = captionids[1];
        var fieldID = fieldids[1];
        var id = ids[1];

        div.id = "divColSizer_" + id;
        div.setAttribute("data-fieldname", divChild.dataset.fieldname);
        div.setAttribute("data-fieldtype", divChild.dataset.fieldtype);
        div.setAttribute("data-columnid", divChild.id);
        div.setAttribute("data-widthchanged", "false");
        div.setAttribute("tabindex","-1")
        div.classList.add("divitem");
        div.style.display = divChild.style.display;
        div.style.position="relative";
        div.style.width = divChild.style.width;
        div.style.overflow = divChild.style.overflow; //"hidden";
        div.style.whiteSpace = divChild.style.whiteSpace; //"nowrap";
        div.style.textOverflow = divChild.style.textOverflow; //"clipped";
        div.style.textAlign = divChildCaption.style.textAlign;
        div.setAttribute("data-tabularcolwidth",divChild.dataset.tabularcolwidth);

        //div.title = divChild.dataset.tabularcolwidth;
        //div.innerHTML = divChildCaption.textContent; //title;
        div.innerText = divChildCaption.textContent; //title;
        div.style.padding = divChildCaption.style.padding; //"5px";
        div.style.fontFamily = divChildCaption.style.fontFamily;
        div.style.fontStyle = divChildCaption.style.fontStyle;
        div.style.fontWeight = divChildCaption.style.fontWeight;
        div.style.fontSize = divChildCaption.style.fontSize;
        div.style.textDecoration = divChildCaption.style.textDecoration;
        div.style.color = divChildCaption.style.color;
        div.style.backgroundColor = divChildCaption.style.backgroundColor;
        div.style.cursor="default";
        //var divCaption = document.createElement("div");

        //divCaption.title = divChildCaption.title;
        //divCaption.id = "sizercaption_" + captionID;
        //divCaption.style.display = "table";
        //divCaption.style.margin = divChildCaption.style.margin; //"5px";
        //divCaption.style.padding = divChildCaption.style.padding; //"5px";
        //divCaption.style.width = divChildCaption.style.width; //"auto";
        //divCaption.style.height = divChildCaption.style.height; //"auto";
        //divCaption.style.textAlign = divChildCaption.style.textAlign; //reportCaptionAlign.toLowerCase();
        //divCaption.textContent = divChildCaption.textContent; //title;
        //divCaption.setAttribute("data-textalign",divChildCaption.dataset.datatextalign);

        //div.appendChild(divCaption);

        return div
    }

}
function createDiv(liId, caption) {
    var ids = liId.split("|");
    var liNode = document.getElementById(ids[0]);
    var li = document.getElementById(ids[0]).cloneNode(true);
    var div = document.createElement("div");
    var reportType = reportView.ReportTemplate;
    var reportCaptionAlign = reportView.ReportCaptionAlign;
    var reportDetailAlign = reportView.ReportDetailAlign;
    var fieldType = li.getAttribute("dragItemTag")
    var itm = reportView.Items.GetItem(ids[0]);
    var fs = JSON.parse(JSON.stringify(fsCaption));
    var ds = JSON.parse(JSON.stringify(dsCaption));

    //div.id = "div_" + liId;
    div.id = "div_" + li.textContent;
    div.setAttribute("draggable", "true");
    div.setAttribute("ondragstart", "dragDiv(event);");
    div.setAttribute("ondrop", "dropOnDiv(event);");
    div.setAttribute("oncontextmenu", "return showMenu(event);")
    div.setAttribute("data-fieldname", li.textContent);
    div.setAttribute("data-fieldtype", fieldType);
    div.setAttribute("tabindex","-1")
    div.classList.add("divitem");

        div.style.display = "inline-block"
        if (reportType == enumReportTemplate.Tabular) {
            div.style.position="relative";
            div.style.width = "1.5in";
            div.style.overflow = "hidden";
            div.style.whiteSpace = "nowrap";
            div.style.textOverflow = "clipped";
            div.setAttribute("data-tabularcolwidth","1.5in");
        }


    var divCaption = document.createElement("div");
    var title = li.textContent;
    
    //fs=fsCaption;
    doSettings(divCaption,fs);
    doDisplaySettings(divCaption,ds);

    if (caption != void 0) {
        var parts = caption.split(":");
         title = parts[0];
    }

    divCaption.title = title;
    divCaption.id = "caption_" + li.textContent;
    divCaption.style.display = "table";
    //divCaption.style.fontFamily = "Tahoma";
    //divCaption.style.fontSize = 12;
    if (reportType == enumReportTemplate.FreeForm) {
        divCaption.style.display = "inline-block";
        divCaption.style.margin = "0px 5px 0px 0px";
        divCaption.style.padding = "5px";
        divCaption.style.width = "auto";
        divCaption.style.cssFloat = "left";
    }
    else {
        divCaption.style.padding = "1px";
        divCaption.style.width = "inherit"; //1.5in
    }
    divCaption.style.height = "auto";
    //divCaption.style.border = "solid 1px blue";
    //divCaption.style.backgroundColor = "lightyellow";
    divCaption.style.textAlign = reportCaptionAlign.toLowerCase();

    if (reportType == enumReportTemplate.Tabular) {
        divCaption.textContent = title;
    }
    else {
        if (itm == void 0 || itm.FieldLayout == enumFieldLayout.Inline)
            divCaption.textContent = title + ": ";
        else if (itm.FieldLayout == enumFieldLayout.Block)
            divCaption.textContent = title;
    }
    
    divCaption.addEventListener("click", onCaptionClick);
    divCaption.setAttribute("data-textalign",reportCaptionAlign);

    div.appendChild(divCaption);

    var divField = document.createElement("div");

    fs = JSON.parse(JSON.stringify(fsDetail));
    ds = JSON.parse(JSON.stringify(dsDetail));

    //fs = fsDetail;
    doSettings(divField,fs);
    doDisplaySettings(divField,ds);

    divField.title = li.textContent;
    divField.id = "field_" + li.textContent;
    divField.style.display = "table";
    //divField.style.fontFamily = "Tahoma";
    //divField.style.fontSize = 12;

    if (reportType == enumReportTemplate.FreeForm) {
        divField.style.display = "inline-block";
        divField.style.margin = "0px 0px 0px 0px ";
        divField.style.padding = "5px";
        divField.style.width = "auto";
    }
    else {
        divField.style.padding = "1px";
        divField.style.width = "inherit";
    }
    divField.style.height = "auto";
    //divField.style.border = "solid 1px black";
    if (reportDetailAlign != "Auto")
      divField.style.textAlign = reportDetailAlign.toLowerCase();
    divField.textContent = li.textContent;
    divField.setAttribute("data-textalign",reportDetailAlign);

    div.appendChild(divField);
    return div;
}

function onCaptionChanged(ev) {
    var trgt = ev.currentTarget;
    var divtb = trgt.parentElement;
    var divtbParent = divtb.parentElement;
    var reportType = reportView.ReportTemplate;
    var fieldFormat = divtbParent.dataset.fieldformat.toUpperCase();
    var left = parseInt(divtbParent.dataset.dropleft,10);
    var top = parseInt(divtbParent.dataset.droptop,10);

    if (trgt.id.startsWith("txtCaption_")) {
        var captionID = trgt.id.substring(11);
        var div = document.createElement("div");
        trgt.removeEventListener("blur", onCaptionChanged);
        trgt.removeEventListener("click", onTBCaptionClick);
        //divtb.title = trgt.value;
        //divtb.textContent = trgt.value;
        //divtb.removeChild(trgt);
        //divtb.id = captionID;

        div.id = captionID;
        div.title = trgt.value;
        div.style.display = "table";
        doSettings(div,fsCaption);
        doDisplaySettings(div,dsCaption);

        if (reportType == enumReportTemplate.FreeForm) {
            div.style.overflow = "hidden";
            div.style.whiteSpace = "nowrap";
            div.style.textOverflow = "clipped";

            if(fieldFormat == "INLINE") {
                div.style.display = "inline-block";
                div.style.margin = "0px 5px 0px 0px";
                div.style.padding = "5px";
                //div.style.width = "auto";
                div.style.cssFloat = "left";
            }
            else {
                div.style.display = "";
                div.style.margin = "5px";
                div.style.padding = "5px";
                div.style.width = parseInt(divClone.dataset.dropwidth) + "px";
                div.style.cssFloat = "";
            }
        }
        else {
            div.style.padding = "1px";
            div.style.width = "inherit"; //1.5in
        }
        div.style.height = "auto";
        div.textContent = trgt.value;
        div.setAttribute("data-textalign",divClone.dataset.textalign);
        div.style.textAlign = divClone.dataset.textalign.toLowerCase();
        div.addEventListener("click", onCaptionClick);

        divtbParent.insertBefore(div, divtb);
        divtbParent.removeChild(divtb);

        var divCaption = divtbParent.children[0];
        var divField = divtbParent.children[1];

        var rctCaption=divCaption.getBoundingClientRect();
        var rctField = divField.getBoundingClientRect();

        divCaption.setAttribute("data-dropwidth", parseInt(rctCaption.width,10));
        divCaption.setAttribute("data-dropheight", parseInt(rctCaption.height,10));
        divCaption.setAttribute("data-dropleft",left);
        divCaption.setAttribute("data-droptop", top);

        if (reportType == enumReportTemplate.FreeForm) {
            divCaption.style.width = divCaption.dataset.dropwidth;
            divCaption.style.height = divCaption.dataset.dropheight;
            divCaption.style.left = divCaption.dataset.dropleft;
            divCaption.top = divCaption.dataset.droptop;
        }

        divField.setAttribute("data-dropwidth", parseInt(rctField.width,10));
        divField.setAttribute("data-dropheight", parseInt(rctField.height,10));

        if (fieldFormat=="INLINE") {
            divField.setAttribute("data-dropleft",parseInt(left+rctCaption.width+3,10))
            divField.setAttribute("data-droptop", top);
        }
        else if (fieldFormat=="BLOCK") {
            divField.setAttribute("data-dropleft", left);
            divField.setAttribute("data-droptop",parseInt(top+rctCaption.height+3,10))
        }
        if (reportType == enumReportTemplate.FreeForm) {
            divField.style.width = divField.dataset.dropwidth;
            divField.style.height = divField.dataset.dropheight;
            divField.style.left = divField.dataset.dropleft;
            divField.top = divField.dataset.droptop;
        }
        if (reportTemplate == enumReportTemplate.Tabular)
            setColWidthBorders(divtbParent.parentElement);

     }

}

function onTBCaptionClick(ev) {
    var trgt = ev.currentTarget;
    //trgt.focus();
    ev.stopPropagation();
}
function editLabel(divLabel) {
    //divLabel_
    if (divLabel != void 0 && divLabel.id.startsWith("divLabel_")) {
        var divText = divLabel.children[0];
        var divTB = divLabel.children[1];
        var txtTB = divTB.children[0];

        txtTB.value = divText.textContent;
        txtTB.style.width = (divLabel.clientWidth-6) + "px";
        divText.style.display = "none";
        divTB.style.display = "";
        txtTB.focus();
        txtTB.setSelectionRange(0, txtTB.value.length);

    }
}

var divClone;
function editCaption(divCapt) {
    if (divCapt != void 0 && divCapt.id.startsWith("caption_")) {
        var parent = divCapt.parentElement;
        var fieldFormat = parent.dataset.fieldformat;
        var divField = parent.children[1];
        var reportType = reportView.ReportTemplate;
        var trgt = divCapt;
        divClone = divCapt.cloneNode(true);

        trgt.removeEventListener("click", onCaptionClick);

        var divCaption = document.createElement("div");
        divCaption.title = trgt.textContent;
        // change this
        divCaption.id = "tbCaption_" + trgt.id;

        divCaption.style.display = "table";
        divCaption.style.fontFamily = "Tahoma";
        divCaption.style.fontSize = 12;
        if (reportType == enumReportTemplate.FreeForm && fieldFormat=="Inline") {
            divCaption.style.margin = "0px 5px 0px 0px";
            divCaption.style.cssFloat = "left";
            divField.style.margin = "0px 0px 0px 0px";
        }
        else {
            //divCaption.style.margin = "5px";
        }
        //divCaption.style.margin = "5px";
        //divCaption.style.padding = "5px";
        divCaption.style.width = "auto";
        //divCaption.style.width = "1.5in"
        divCaption.style.height = "auto";
        divCaption.style.border = "none";
        //divCaption.style.border = "solid 1px blue";
        divCaption.style.backgroundColor = "lightyellow";
        var txtbox = document.createElement("INPUT");
        txtbox.setAttribute("type", "text");
        txtbox.setAttribute("value", trgt.textContent);
        txtbox.id = "txtCaption_" + trgt.id;
        txtbox.addEventListener("blur", onCaptionChanged);
        txtbox.addEventListener("click", onTBCaptionClick);
        //txtbox.setAttribute("onblur","onCaptionChanged(event);")
        //txtbox.setAttribute("autopostback", "false");
        //txtbox.style.border = "solid 1px transparent";
        txtbox.style.borderLeft = "solid 1px gray";
        txtbox.style.borderBottom = "solid 1px gray";

        txtbox.style.outline = "none";
        
        //txtbox.style.border = "solid 1px blue";
        if (reportType == enumReportTemplate.FreeForm ) {
            txtbox.style.width = trgt.clientWidth + 10 + "px";
            txtbox.style.height = trgt.clientHeight;
        }
        else
            txtbox.style.width = "1.5in";

        txtbox.style.fontSize = 12
        txtbox.style.backgroundColor = "lightyellow";
        ;
        divCaption.appendChild(txtbox);
        parent.insertBefore(divCaption, trgt);
        parent.removeChild(trgt)
        txtbox.focus();
        txtbox.setSelectionRange(0, txtbox.value.length);

    }
}
function onCaptionClick(ev) {
    editCaption(ev.currentTarget);
    ev.stopPropagation();
}

function dropOnDiv(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    var params = data.split(",");
    var dropId = params[0];
    var offsetX = params[1];
    var offsetY = params[2];
    var reportType = reportView.ReportTemplate;
    var divToDrop = "";
    var trgt = ev.currentTarget;
    var parent = trgt.offsetParent;
    var targRect = parent.getBoundingClientRect();
    var clientRect = trgt.getBoundingClientRect();
    var targId = trgt.id;
    var child = ev.target
    var curX = ev.offsetX - offsetX;
    var curY = ev.offsetY - offsetY;
    var bnew = true;

    if (trgt.id != child.id) {
        curX = child.offsetLeft + curX;
        curY = child.offsetTop + curY;
    }

    if (dropId.startsWith("div_")) {
        divToDrop = document.getElementById(dropId);
        bnew = false;
    }
    else if (dropId.startsWith("lstFields_")) {
        if (!fieldExists(dropId)) {
            divToDrop = createDiv(dropId);
            if (reportType == enumReportTemplate.FreeForm) {
                divToDrop.setAttribute("data-fieldformat","Inline");
            }
            else {
                divToDrop.setAttribute("data-fieldformat","Block");
            }
            //var fieldno = parent.children.length-1
            //divToDrop.setAttribute("data-fieldindex", fieldno);
        }
        else {
            var li = document.getElementById(dropId);
            var fname = li.textContent;
            var msg = fname + " is already defined to the report."
            showMessage(msg, "Field Already Defined", enumMessageType.Warning);
            return;
        }

    }
    else {
        ev.stopPropagation();
        return false;
    }

    if (dropId != targId) {
        //parent.insertBefore(divToDrop, trgt.nextSibling); //insert after
        parent.insertBefore(divToDrop, trgt); //insert before
        reindexItems(parent);
        resetItemsSize(parent);
        setColWidthBorders(parent);
    }

    if (reportType == enumReportTemplate.FreeForm) {
        var left = parseInt(divToDrop.style.left, 10) + curX;
        var top = parseInt(divToDrop.style.top, 10) + curY

        divToDrop.style.position = "absolute";
        if (bnew) {
            divToDrop.style.left = trgt.offsetLeft;
            divToDrop.style.top = trgt.offsetTop + trgt.offsetHeight + 5;
            //adjustFieldsAfter(parent,divToDrop);
        }
        else if (dropId != targId) {
            divToDrop.style.left = (trgt.offsetLeft + curX) + "px"; //ev.clientX-ev.target.offsetLeft; //ev.x-ev.target.offsetLeft;
            divToDrop.style.top = (trgt.offsetTop + curY) + "px"; //ev.clientY-ev.target.offsetTop; //ev.y-ev.target.offsetTop;
        }
        else {
            divToDrop.style.left = left + "px";
            divToDrop.style.top = top + "px";
        }
        left = parseInt(divToDrop.style.left);
        top = parseInt(divToDrop.style.top);
        divToDrop.setAttribute("data-dropleft", parseInt(left,10));
        divToDrop.setAttribute("data-droptop", parseInt(top,10));

        var divCaption = divToDrop.children[0];
        var divField = divToDrop.children[1];
        var rctCaption = divCaption.getBoundingClientRect();
        var rctField = divField.getBoundingClientRect();
        var fieldFormat = divToDrop.dataset.fieldformat.toUpperCase();

        divCaption.setAttribute("data-dropleft",left,10);
        divCaption.setAttribute("data-droptop",top);
        
        if (fieldFormat=="INLINE") {
            divField.setAttribute("data-dropleft",parseInt(left+rctCaption.width+3,10))
            divField.setAttribute("data-droptop", top);
        }
        else if (fieldFormat=="BLOCK") {
            divField.setAttribute("data-dropleft", left);
            divField.setAttribute("data-droptop",parseInt(top+rctCaption.height+3,10))
        }

    }
      
    ev.stopPropagation();
}
function adjustParentWidth(divToAdd, parent) {
    var parentRect = parent.getBoundingClientRect();
    var parentWidth = parentRect.width;
    var divRect = divToAdd.getBoundingClientRect()
    var divWidth = divRect.width;
    var totalWidth = 0

    //parent.style.width=parentWidth + divWidth + "px"
}
function reindexBeforeInsert(divindex) {
    var divDrop = document.getElementById("divDrop");
    var div;
    for (var i = 0; i < divDrop.children.length; i++) {
        div = divDrop.children[i];
        if (div.id.startsWith("div")) {
            var childindex = parseInt(div.dataset.fieldindex,10);
            if (childindex >= divindex) div.setAttribute("data-fieldindex", childindex + 1);
        }
    }
}
function resetItemsSize(target) {
    var div;
    var j = 0;
    var divRect;
    var targRect = target.getBoundingClientRect(target);

    for (var i = 0; i < target.children.length; i++) {
        div = target.children[i];
        if (div.id.startsWith("div_")) {
            var divCaption = div.children[0];
            var divField = div.children[1];
            var rctCaption=divCaption.getBoundingClientRect();
            var rctField = divField.getBoundingClientRect();
            var fieldFormat = div.dataset.fieldformat.toUpperCase();

            divRect = div.getBoundingClientRect();
            var left = parseInt(divRect.left-targRect.left,10);
            var top = parseInt(divRect.top-targRect.top,10)

            div.setAttribute("data-dropleft", parseInt(left,10));
            div.setAttribute("data-droptop", parseInt(top,10));
            div.setAttribute("data-dropwidth", parseInt(divRect.width,10));
            div.setAttribute("data-dropheight", parseInt(divRect.height,10));
            
            divCaption.setAttribute("data-dropwidth", parseInt(rctCaption.width,10));
            divCaption.setAttribute("data-dropheight", parseInt(rctCaption.height,10));
            divCaption.setAttribute("data-dropleft",left);
            divCaption.setAttribute("data-droptop", top);

            
            divField.setAttribute("data-dropwidth", parseInt(rctField.width,10));
            divField.setAttribute("data-dropheight", parseInt(rctField.height,10));

            
            if (fieldFormat=="INLINE") {
                divField.setAttribute("data-dropleft",parseInt(left+rctCaption.width+3,10))
                divField.setAttribute("data-droptop", top);
            }
            else if (fieldFormat=="BLOCK") {
                divField.setAttribute("data-dropleft", left);
                divField.setAttribute("data-droptop",parseInt(top+rctCaption.height+3,10))
            }
        }
    }
}
function reindexItems(target) {
    var div;
    var j = 1;

    for (var i = 0; i < target.children.length; i++) {
        div = target.children[i];
        if (div.id.startsWith("div")) {
            div.setAttribute("data-fieldindex", j);
            j = j+1;
        }
    }
}
function dropOnHeaderFooter(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    var params = data.split(",");
    var id = params[0];
    var offsetX = params[1];
    var offsetY = params[2];
    var targRect = ev.target.getBoundingClientRect();
    var top = ev.offsetY - (offsetY);
    var left = ev.offsetX - (offsetX);
    var n;
    var divRect;
    var divCaption;
    if (id.startsWith("div")) {
        var div = document.getElementById(id);
        div.style.left = left + "px";
        div.style.top = top + "px";

        div.setAttribute("data-dropleft", parseInt(left,10));
        div.setAttribute("data-droptop", parseInt(top,10));
    }
    else {
        var fieldno; // = ev.target.children.length-2;
        var li = document.getElementById(id).cloneNode(true);
        var di_id = li.getAttribute("dragitemID");
        var caption = li.textContent;
        if (di_id=="Label") {
            n = parseInt(getLabelCount() + 1, 10);
            fsLabel=fsCaption;
            var divLabel=createLabel(n);
            ev.target.appendChild(divLabel);

            fieldno = ev.target.children.length;
            divLabel.setAttribute("data-fieldindex", fieldno);
            divLabel.setAttribute("data-textalign","Left");

            divLabel.style.textAlign = "left";
            divLabel.style.position = "absolute";
            divLabel.style.left = left + "px";
            divLabel.style.top = top + "px";

            divCaption = divLabel.children[0];
            divCaption.style.textAlign = "left";

            divRect=divLabel.getBoundingClientRect();
            divLabel.setAttribute("data-dropleft", parseInt(left,10));
            divLabel.setAttribute("data-droptop", parseInt(top,10));
            divLabel.setAttribute("data-dropwidth", parseInt(divRect.width,10));
            divLabel.setAttribute("data-dropheight", parseInt(divRect.height,10));
        }
        else {
            n = parseInt(getSpecialFieldCount(di_id) + 1, 10);

            
            var divField = createSpecialField(di_id,caption,n);
            divField.setAttribute("data-itemtype",di_id);

            ev.target.appendChild(divField);

            fieldno = ev.target.children.length;
            divField.setAttribute("data-fieldindex", fieldno);
            divField.setAttribute("data-textalign","Left");

            divField.style.textAlign = "left";
            divField.style.position = "absolute";
            divField.style.left = left + "px";
            divField.style.top = top + "px";

            divRect=divField.getBoundingClientRect();
            divField.setAttribute("data-dropleft", parseInt(left,10));
            divField.setAttribute("data-droptop", parseInt(top,10));
            divField.setAttribute("data-dropwidth", parseFloat(divRect.width).toFixed());
            divField.setAttribute("data-dropheight", parseFloat(divRect.height).toFixed());
        }
    }

}
function onDrop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    var params = data.split(",");
    var id = params[0];
    var offsetX = params[1];
    var offsetY = params[2];
    var targRect = ev.target.getBoundingClientRect();
    var top = ev.offsetY - (offsetY);
    var left = ev.offsetX - (offsetX);
    var divCaption;
    var divField;
    var rctCaption;
    var rctField;
    var divRect;
    var n;

    if (id.startsWith("div_")) {
        var div = document.getElementById(id);
        var fieldFormat = div.dataset.fieldformat.toUpperCase();

        if (reportView.ReportTemplate == enumReportTemplate.FreeForm) {
            div.style.position = "absolute";
            div.setAttribute("data-dropleft", left);
            div.setAttribute("data-droptop", top);
            div.style.left = left + "px"; 
            div.style.top = top + "px";

            divCaption = div.children[0];
            divField = div.children[1];
            rctCaption=divCaption.getBoundingClientRect();
            rctField = divField.getBoundingClientRect();

            divRect = div.getBoundingClientRect();

            divCaption.setAttribute("data-dropwidth", parseInt(rctCaption.width,10));
            divCaption.setAttribute("data-dropheight", parseInt(rctCaption.height,10));
            divCaption.setAttribute("data-dropleft",left);
            divCaption.setAttribute("data-droptop", top);

            divField.setAttribute("data-dropwidth", parseInt(rctField.width,10));
            divField.setAttribute("data-dropheight", parseInt(rctField.height,10));

            if (fieldFormat=="INLINE") {
                divField.setAttribute("data-dropleft",parseInt(left+rctCaption.width+3,10))
                divField.setAttribute("data-droptop", top);
            }
            else if (fieldFormat=="BLOCK") {
                divField.setAttribute("data-dropleft", left);
                divField.setAttribute("data-droptop",parseInt(top+rctCaption.height+3,10))
            }
        }
        else {
            deleteField(div);
            ev.target.appendChild(div);
            reindexItems(ev.target);
            resetItemsSize(ev.target);
            setColWidthBorders(ev.target);
        }
    }
    else if (id.startsWith("divLabel_")) {
        var div = document.getElementById(id);
        divRect = div.getBoundingClientRect();

        div.setAttribute("data-dropleft", left);
        div.setAttribute("data-droptop", top);
        div.setAttribute("data-dropwidth", parseInt(divRect.width,10));
        div.setAttribute("data-dropheight", parseInt(divRect.height,10));

        div.style.left = left + "px";
        div.style.top = top + "px";
    }
    else if (id.startsWith("lstFields_")) {
        if (!fieldExists(id)) {
            if ( reportView.ReportTemplate == enumReportTemplate.FreeForm) {
                var ids = id.split("|");
                var liNode = document.getElementById(ids[0]);

                // Add label to free form report
                if (liNode.textContent == "** Label **") {
                    n = parseInt(getLabelCount() + 1, 10);
                    fsLabel=fsCaption;
                    var divLabel=createLabel(n);
                    ev.target.appendChild(divLabel);

                    fieldno = ev.target.children.length;
                    divLabel.setAttribute("data-fieldindex", fieldno);
                    divLabel.setAttribute("data-textalign","Left");

                    divLabel.style.textAlign = "left";
                    divLabel.style.position = "absolute";
                    divLabel.style.left = left + "px";
                    divLabel.style.top = top + "px";

                    divCaption = divLabel.children[0];
                    divCaption.style.textAlign = "left";

                    divRect=divLabel.getBoundingClientRect();
                    divLabel.setAttribute("data-dropleft", parseInt(left,10));
                    divLabel.setAttribute("data-droptop", parseInt(top,10));
                    divLabel.setAttribute("data-dropwidth", parseInt(divRect.width,10));
                    divLabel.setAttribute("data-dropheight", parseInt(divRect.height,10));

                    return;
                }
            }
            var div = createDiv(id);
            ev.target.appendChild(div);
            var fieldno = ev.target.children.length-1
            div.setAttribute("data-fieldindex", fieldno);
            
            divCaption = div.children[0];
            divField = div.children[1];
            rctCaption=divCaption.getBoundingClientRect();
            rctField = divField.getBoundingClientRect();

            divRect = div.getBoundingClientRect();
            if (divRect.height > 0) {
                div.setAttribute("data-dropwidth", parseFloat(divRect.width).toFixed());
                div.setAttribute("data-dropheight", parseInt(divRect.height,10));
            }
            if (reportView.ReportTemplate == enumReportTemplate.FreeForm) {

                div.setAttribute("data-fieldformat", "Inline");
                div.style.position = "absolute";
                div.setAttribute("data-dropleft", left);
                div.setAttribute("data-droptop", top);
                div.style.left = left + "px";
                div.style.top = top + "px";

                divCaption.setAttribute("data-dropwidth", parseFloat(rctCaption.width).toFixed());
                divCaption.setAttribute("data-dropheight", parseInt(rctCaption.height,10));
                divCaption.setAttribute("data-dropleft",left);
                divCaption.setAttribute("data-droptop", top);
                divCaption.style.width = divCaption.dataset.dropwidth;

                divField.setAttribute("data-dropwidth", parseFloat(rctField.width).toFixed());
                divField.setAttribute("data-dropheight", parseInt(rctField.height,10));
                divField.setAttribute("data-dropleft",parseInt(left+rctCaption.width+3,10))
                divField.setAttribute("data-droptop", top);
                divField.style.width = divField.dataset.dropwidth;
            }
            else {
                left=parseInt(divRect.left-targRect.left,10)
                top=parseInt(divRect.top-targRect.top,10)

                div.setAttribute("data-fieldformat", "Block");
                div.setAttribute("data-dropleft", parseInt(left,10));
                div.setAttribute("data-droptop", parseInt(top,10));

                divCaption.setAttribute("data-dropwidth", parseFloat(rctCaption.width).toFixed());
                divCaption.setAttribute("data-dropheight", parseInt(rctCaption.height,10));
                divCaption.setAttribute("data-dropleft",left);
                divCaption.setAttribute("data-droptop", top);

                divField.setAttribute("data-dropwidth", parseFloat(rctField.width).toFixed());
                divField.setAttribute("data-dropheight", parseInt(rctField.height,10));
                divField.setAttribute("data-dropleft", left);
                divField.setAttribute("data-droptop",parseInt(top+rctCaption.height+3,10))
                setColWidthBorders(ev.target);
            }

        }
        else {
            var li = document.getElementById(id);
            var fname = li.textContent;
            var msg = fname + " is already defined to the report."
            showMessage(msg, "Field Already Defined", enumMessageType.Warning);
        }
     }
}

function applyHeaderFooterFieldSettings(id) {
    var divHeader = document.getElementById("divHeader");
    var divFooter = document.getElementById("divFooter");
    var fs;
    var ds;
    var field;

    if (id == "HeaderFieldSettings") {
        fs = fsHeaderFieldSettings;
        ds = dsHeaderFieldSettings;
        div = divHeader;
    }
    else if (id == "FooterFieldSettings") {
        fs = fsFooterFieldSettings;
        ds = dsFooterFieldSettings;
        div = divFooter;
    }
    
    if (div != void 0 && div != undefined) {
        for (var i=0; i<div.children.length; i++) {
            field = div.children[i];

            field.style.fontFamily = fs.FontName;

            if (fs.FontStyle == "Regular") {
                field.style.fontStyle = "normal";
                field.style.fontWeight = "normal";
            }
            else if (fs.FontStyle == "Italic") {
                field.style.fontStyle = "italic";
                field.style.fontWeight = "normal";
            }
            else if (fs.FontStyle == "Bold" ) {
                field.style.fontStyle = "normal";
                field.style.fontWeight = "bold";

            }
            else if (fs.FontStyle == "Bold Italic") {
               field.style.fontStyle = "italic";
               field.style.fontWeight= "bold";
            }

            field.style.fontSize=fs.FontSize;

            if (fs.Strikeout && fs.Underline)
                field.style.textDecoration = "underline line-through"
            else if (fs.Strikeout) 
                field.style.textDecoration = "line-through";
            else if (fs.Underline)
                field.style.textDecoration = "underline";
            else
                field.style.textDecoration = "none";

            field.style.color=fs.ColorName;
            var divRect = field.getBoundingClientRect();
            if (parseInt(divRect.width,10) > 0) {
                field.setAttribute("data-dropwidth", parseInt(divRect.width,10));
                field.setAttribute("data-dropheight", parseInt(divRect.height,10));
            }

            // display settings
           field.style.backgroundColor = ds.BackColorName;
           field.style.borderColor = ds.BorderColorName;
           field.style.borderStyle = ds.BorderStyle.toLowerCase();
           field.style.borderWidth =ds.BorderWidth + "px";

           field.setAttribute("data-backcolor",ds.BackColorName);
           field.setAttribute("data-bordercolor",ds.BorderColorName);
           field.setAttribute("data-borderstyle",ds.BorderStyle);
           field.setAttribute("data-borderwidth",ds.BorderWidth);

       }

    }
}

function applyHeaderFooterSettings(section) {
    var ds = new HeaderFooterSettings();
    var divHeaderDisplay = document.getElementById("divHeaderDisplay");
    var divHeader = document.getElementById("divHeader");
    var divFooterDisplay = document.getElementById("divFooterDisplay");
    var divFooter = document.getElementById("divFooter");
    var border;

    if (section == "header") {
        ds =JSON.parse(JSON.stringify(dsHeader));
        if  (Number.isNaN(Number.parseFloat(ds.Height)))
            ds.Height = "1";

        reportView.HeaderHeight = ds.Height;
        reportView.HeaderBackColor = ds.BackColorName;
        reportView.HeaderBorderColor = ds.BorderColorName;
        reportView.HeaderBorderStyle = ds.BorderStyle;
        reportView.HeaderBorderWidth = ds.BorderWidth;

        divHeaderDisplay.setAttribute("data-backcolor",ds.BackColorName);
        divHeaderDisplay.setAttribute("data-bordercolor",ds.BorderColorName);
        divHeaderDisplay.setAttribute("data-borderstyle",ds.BorderStyle);
        divHeaderDisplay.setAttribute("data-borderwidth",parseInt(ds.BorderWidth,10));
        divHeaderDisplay.setAttribute("data-height",parseFloat(ds.Height));

        border = ds.BorderWidth + "px " + ds.BorderStyle + " " + ds.BorderColorName;
        divHeader.style.backgroundColor = ds.BackColorName;
        divHeader.style.height = ds.Height + "in";
        divHeader.style.border  = border;

    }
    else if(section == "footer") {
        ds =JSON.parse(JSON.stringify(dsFooter));
        if  (Number.isNaN(Number.parseFloat(ds.Height)))
            ds.Height = "1";
        reportView.FooterHeight = ds.Height;
        reportView.FooterBackColor = ds.BackColorName;
        reportView.FooterBorderColor = ds.BorderColorName;
        reportView.FooterBorderStyle = ds.BorderStyle;
        reportView.FooterBorderWidth = ds.BorderWidth;

        divFooterDisplay.setAttribute("data-backcolor",ds.BackColorName);
        divFooterDisplay.setAttribute("data-bordercolor",ds.BorderColorName);
        divFooterDisplay.setAttribute("data-borderstyle",ds.BorderStyle);
        divFooterDisplay.setAttribute("data-borderwidth",parseInt(ds.BorderWidth,10));
        divFooterDisplay.setAttribute("data-height",parseFloat(ds.Height));

        border = ds.BorderWidth + "px " + ds.BorderStyle + " " + ds.BorderColorName;
        divFooter.style.backgroundColor = ds.BackColorName;
        divFooter.style.height = ds.Height + "in";
        divFooter.style.border  = border;


    }

}
function applyDisplaySettings(id) {
    var ds = new DisplaySettings();
    var divDrop = document.getElementById("divDrop");
    var div;
    var divCaption;
    var divDetail;

    if (id == "DetailSettings") {
        ds = JSON.parse(JSON.stringify(dsDetail));

        reportView.DataBackColor = ds.BackColorName;
        reportView.DataBorderColor = ds.BorderColorName;
        reportView.DataBorderStyle = ds.BorderStyle;
        reportView.DataBorderWidth = ds.BorderWidth;


    }
    else if (id == "CaptionSettings") {
        ds = JSON.parse(JSON.stringify(dsCaption));

        reportView.LabelBackColor = ds.BackColorName;
        reportView.LabelBorderColor = ds.BorderColorName;
        reportView.LabelBorderStyle = ds.BorderStyle;
        reportView.LabelBorderWidth = ds.BorderWidth;

    }

    for (var i = 0; i < divDrop.children.length; i++) {
        div = divDrop.children[i];
        if (div.id.startsWith("div_")) {
            divCaption = div.children[0];
            divDetail = div.children[1];
            if (id == "CaptionSettings") {
                divCaption.style.backgroundColor = ds.BackColorName;
                divCaption.style.borderColor = ds.BorderColorName;
                divCaption.style.borderStyle = ds.BorderStyle.toLowerCase();
                divCaption.style.borderWidth = ds.BorderWidth + "px";

                divCaption.setAttribute("data-backcolor",ds.BackColorName);
                divCaption.setAttribute("data-bordercolor",ds.BorderColorName);
                divCaption.setAttribute("data-borderstyle",ds.BorderStyle);
                divCaption.setAttribute("data-borderwidth",parseInt(ds.BorderWidth,10));
            }
            else if (id == "DetailSettings") {
                divDetail.style.backgroundColor = ds.BackColorName;
                divDetail.style.borderColor = ds.BorderColorName;
                divDetail.style.borderStyle = ds.BorderStyle.toLowerCase();
                divDetail.style.borderWidth = ds.BorderWidth + "px";

                divDetail.setAttribute("data-backcolor",ds.BackColorName);
                divDetail.setAttribute("data-bordercolor",ds.BorderColorName);
                divDetail.setAttribute("data-borderstyle",ds.BorderStyle);
                divDetail.setAttribute("data-borderwidth",parseInt(ds.BorderWidth,10));

            }
        }
    }

}
function applySettings(id) {
    var fs = new FontSettings();
    var divDrop = document.getElementById("divDrop");
    var divHeader = document.getElementById("divHeader");
    var divFooter = document.getElementById("divFooter");

    if (id == "DetailSettings") {
        fs = fsDetail;
        reportView.DataFontName = fs.FontName;
        reportView.DataFontSize = fs.FontSize;
        reportView.DataForeColor = fs.ColorName;
        reportView.DataFontStyle = fs.FontStyle;
        reportView.DataUnderline = fs.Underline;
        reportView.DataStrikeout = fs.Strikeout;
        
        var div;
        for (var i = 0; i < divDrop.children.length; i++) {
            div = divDrop.children[i];
            if (div.id.startsWith("div_")) {
                div = div.children[1];
                div.style.fontFamily = fs.FontName;
                if (fs.FontStyle == "Regular") {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontStyle == "Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontStyle == "Bold" ) {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "bold";

                }
                else if (fs.FontStyle == "Bold Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight= "bold";
                }
                div.style.fontSize=fs.FontSize;

                if (fs.Strikeout && fs.Underline)
                    div.style.textDecoration = "underline line-through"
                else if (fs.Strikeout) 
                    div.style.textDecoration = "line-through";
                else if (fs.Underline)
                    div.style.textDecoration = "underline";
                else
                    div.style.textDecoration = "none";

                div.style.color=fs.ColorName;

            }
        }
/*
        for (var i =0; i < divHeader.children.length; i++) {
            div = divHeader.children[i];
            if (div.id.startsWith("div") && !div.id.startsWith("divLabel_")) {
                div.style.fontFamily = fs.FontName;
                if (fs.FontStyle == "Regular") {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontStyle == "Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontSyle == "Bold" ) {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "bold";

                }
                else if (fs.FontStyle == "Bold Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight= "bold";
                }
                div.style.fontSize=fs.FontSize;

                if (fs.Strikeout && fs.Underline)
                    div.style.textDecoration = "underline line-through"
                else if (fs.Strikeout) 
                    div.style.textDecoration = "line-through";
                else if (fs.Underline)
                    div.style.textDecoration = "underline";
                else
                    div.style.textDecoration = "none";

                div.style.color=fs.ColorName;

            }
        }

        for (var i =0; i < divFooter.children.length; i++) {
            div = divFooter.children[i];
            if (div.id.startsWith("div") && !div.id.startsWith("divLabel_")) {
                div.style.fontFamily = fs.FontName;
                if (fs.FontStyle == "Regular") {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontStyle == "Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontSyle == "Bold" ) {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "bold";

                }
                else if (fs.FontStyle == "Bold Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight= "bold";
                }
                div.style.fontSize=fs.FontSize;

                if (fs.Strikeout && fs.Underline)
                    div.style.textDecoration = "underline line-through"
                else if (fs.Strikeout) 
                    div.style.textDecoration = "line-through";
                else if (fs.Underline)
                    div.style.textDecoration = "underline";
                else
                    div.style.textDecoration = "none";

                div.style.color=fs.ColorName;

            }
        }
        */
    }
    else if (id == "CaptionSettings") {
        fs = fsCaption;

        reportView.LabelFontName = fs.FontName;
        reportView.LabelFontSize = fs.FontSize;
        reportView.LabelForeColor = fs.ColorName;
        reportView.LabelFontStyle = fs.FontStyle;
        reportView.LabelUnderline = fs.Underline;
        reportView.LabelStrikeout = fs.Strikeout;

        for (var i = 0; i < divDrop.children.length; i++) {
            var div = divDrop.children[i];
            if (div.id.startsWith("div_")) {
                div = div.children[0];
                div.style.fontFamily = fs.FontName;
                if (fs.FontStyle == "Regular") {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontStyle == "Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight = "normal";
                }
                else if (fs.FontStyle == "Bold" ) {
                    div.style.fontStyle = "normal";
                    div.style.fontWeight = "bold";

                }
                else if (fs.FontStyle == "Bold Italic") {
                    div.style.fontStyle = "italic";
                    div.style.fontWeight= "bold";
                }
                div.style.fontSize=fs.FontSize;

                if (fs.Strikeout && fs.Underline)
                    div.style.textDecoration = "underline line-through"
                else if (fs.Strikeout) 
                    div.style.textDecoration = "line-through";
                else if (fs.Underline)
                    div.style.textDecoration = "underline";
                else
                    div.style.textDecoration = "none";

                div.style.color=fs.ColorName;

            }
        }
    }
}
function cancelHeaderFooterDlg(popup) {
    var divHdrFtrChooseBackColor = document.getElementById("divHeaderFooterChooseBackColor");
    var divHdrFtrChooseBorderColor = document.getElementById("divHeaderFooterChooseBorderColor");
    var btnHdrFtrBackColorCustomize = document.getElementById("btnHeaderFooterBackColorCustomize");
    var btnHdrFtrBorderColorCustomize = document.getElementById("btnHeaderFooterBorderColorCustomize");

    divHdrFtrChooseBackColor.removeEventListener("click",onChooseColorClicked);
    divHdrFtrChooseBorderColor.removeEventListener("click",onChooseColorClicked);
    btnHdrFtrBackColorCustomize.removeEventListener("click",onCustomColorClick);
    btnHdrFtrBorderColorCustomize.removeEventListener("click",onCustomColorClick);
    ckSetTextColors.removeEventListener("change",onHeaderFooterCheckboxChanged);
    closePopup(popup);
}

function removeBorders(section) {
    if (section != void 0 && section != undefined) {
        var div = document.getElementById(section);
        var field;
        for (var i=0; i<div.children.length; i++) {
            field = div.children[i];
            field.dataset.borderstyle = "None";
            field.style.borderStyle = "none";
        }
    }
}
function fillFields(section,colorName) {
    if (section != void 0 && section != undefined) {
        var div = document.getElementById(section);
        var field;

        if (div != void 0 && div != undefined) {
            if (colorName == void 0 || colorName == undefined || colorName == "")
                colorName = "white"
            for (var i=0; i<div.children.length; i++) {
                field = div.children[i];
                field.dataset.backcolor = colorName;
                field.style.backgroundColor = colorName;
            }
        }
    }
}

function setFieldsTextColor(section,colorName) {
    if (section != void 0 && section != undefined) {
        var div = document.getElementById(section);
        var field;

        if (div != void 0 && div != undefined) {
            if (colorName == void 0 || colorName == undefined || colorName == "")
                colorName = "black"
            for (var i=0; i<div.children.length; i++) {
                field = div.children[i];
                field.style.color = colorName;
            }
        }
    }
}
function saveImageFieldData(popup) {
    var msg;

    if (fieldSettings.ImageWidth == "") {
        msg = "Image Sizes are not set."
        showMessage(msg, "Alert", enumMessageType.Warning); 
        return false;
    }
    if (fieldSettings.ImagePath == "" && fieldSettings.ImageData == "") {
        msg = "No Image has been selected.";
        showMessage(msg, "Alert", enumMessageType.Warning);
        return false;
    }
    if (popup != void 0) {
        var divPopupDisplay = document.getElementById(popup);

        removeRadioButtonEvents();
        removeOtherEvents();
        doImageSettings(CurrentField, fieldSettings);
        divPopupDisplay.style.display = "none";
    }
}

function saveHeaderFooterData(popup) {
    var section = CurrentField.id;
    var lstBorderStyles = document.getElementById("lstHeaderFooterBorderStyles");
    var txtBorderWidth = document.getElementById("txtHeaderFooterBorderWidth");
    var divBackColorName = document.getElementById("divHeaderFooterBackColorName");
    var divBorderColorName = document.getElementById("divHeaderFooterBorderColorName");
    var divFieldTextColorName = document.getElementById("divFieldTextColorName");
    var divShowFieldTextColor = document.getElementById("divShowFieldTextColor");
    var divChooseColor = document.getElementById("divHeaderFooterChooseColor");
    var btnCustomize = document.getElementById("btnHeaderFooterCustomize");
    var txtHeight = document.getElementById("txtHeaderFooterHeight");
    var height = document.getElementById("txtHeaderFooterHeight").value;
    var ckFillFields = document.getElementById("ckHeaderFooterFillFields");
    var bFillFields = ckFillFields.checked;
    var ckRemoveBorders = document.getElementById("ckRemoveBorders");
    var bRemoveBorders = ckRemoveBorders.checked;
    var ckSetTextColors = document.getElementById("ckSetTextColors");
    var bSetTextColors = ckSetTextColors.checked;

    var title ="Header Settings";

    if (section == "FooterSettings")
        title = "Footer Settings"

    var borderColor = divBorderColorName.textContent;
    var backColor = divBackColorName.textContent;
    var textColor = divFieldTextColorName.textContent;
    var borderStyle = lstBorderStyles.value;
    var borderWidth = txtBorderWidth.value;

    var ds = new HeaderFooterSettings();

    ds.Title = title;
    ds.Height = height;
    ds.BorderStyle = borderStyle;
    ds.BorderWidth = borderWidth;
    ds.BackColorName = backColor;
    ds.FieldSettings.UseBackColor = bFillFields;
    ds.FieldSettings.RemoveBorders = bRemoveBorders;
    ds.FieldSettings.UseTextColor = bSetTextColors;

    if (bSetTextColors) {
        ds.FieldSettings.TextColorName = textColor;
        if (!textColor.startsWith("#"))
            ds.FieldSettings.TextColorHex = toHex(textColor);
        else
            ds.FieldSettings.TextColorHex = textColor;

        divShowFieldTextColor.style.backgroundColor = textColor;
    }
    else  {
        divFieldTextColorName.textContent = "black";
        divShowFieldTextColor.style.backgroundColor = "black";
    }
        
 
    if (!backColor.startsWith("#")) 
        ds.BackColorHex = toHex(backColor);
    else
        ds.BackColorHex = backColor;

    ds.FieldSettings.BackColorName = ds.BackColorName
    ds.FieldSettings.BackColorHex = ds.BackColorHex;

    ds.BorderColorName = borderColor;

    if (!borderColor.startsWith("#")) 
        ds.BorderColorHex = toHex(borderColor);
    else
        ds.BorderColorHex = borderColor;

    var section = "divHeader";
    if (title == "Header Settings") {
        dsHeader = ds;
        dsHeaderFieldSettings.BackColorName = dsHeader.BackColorName;
        applyHeaderFooterSettings("header");
    }
    else if (title == "Footer Settings") {
        dsFooter = ds;
        dsFooterFieldSettings.BackColorName = dsFooter.BackColorName;
        applyHeaderFooterSettings("footer");
        section = "divFooter";
    }
    if (bFillFields)
        fillFields(section, ds.BackColorName);
    if (bSetTextColors)
        setFieldsTextColor(section,textColor);
    if (bRemoveBorders)
        removeBorders(section);

    cancelHeaderFooterDlg(popup);
    showSection(section);
}

function saveFontData(popup) {
    var lstFonts = document.getElementById("lstFonts");
    var lstFontStyles = document.getElementById("lstFontStyles");
    var lstFontSize = document.getElementById("lstFontSize");
    var lstBorderStyles = document.getElementById("lstBorderStyles");
    var txtBorderWidth = document.getElementById("txtBorderWidth");
    var ckStrikeout = document.getElementById("ckStrikeout");
    var ckUnderline = document.getElementById("ckUnderline");
    var divColorName = document.getElementById("divColorName");
    var divBackColorName = document.getElementById("divBackColorName");
    var divBorderColorName = document.getElementById("divBorderColorName");
    var divChooseColor = document.getElementById("divChooseColor");
    var btnCustomize = document.getElementById("btnCustomize");
    var showDetails = false;

    var fs = new FontSettings();
    var fontName = lstFonts.value;
    var fontStyle = lstFontStyles.value;
    var fontSize = lstFontSize.value;
    var strikeout = ckStrikeout.checked;
    var underline=ckUnderline.checked;
    var fontColor=divColorName.textContent;

    if (fontName != void 0 && fontName != "")
        fs.FontName=fontName;
    if (fontStyle != void 0 && fontStyle != "")
        fs.FontStyle=fontStyle;
    if (fontSize != void 0 && fontSize !="")
        fs.FontSize = fontSize + "px";
    fs.Strikeout=strikeout;
    fs.Underline = underline;
    if (!fontColor.startsWith("#")) {
        fs.ColorName = fontColor;
        fs.ColorHex = toHex(fontColor);
    }
    else {
        fs.ColorName = fontColor;
        fs.ColorHex = fontColor;
    }

    var borderColor = divBorderColorName.textContent;
    var backColor = divBackColorName.textContent;
    var borderStyle = lstBorderStyles.value;
    var borderWidth = txtBorderWidth.value;

    var ds = new DisplaySettings();
    ds.BorderStyle = borderStyle;
    ds.BorderWidth = borderWidth;
    ds.BackColorName = backColor;
    if (!backColor.startsWith("#")) 
        ds.BackColorHex = toHex(backColor);
    else
        ds.BackColorHex = backColor;
    ds.BorderColorName = borderColor;
    if (!borderColor.startsWith("#")) 
        ds.BorderColorHex = toHex(borderColor);
    else
        ds.BorderColorHex = borderColor;


    if (CurrentField != null) {
        if (CurrentField.id.startsWith("div")) {
            var divText = CurrentField; //CurrentField.children[0];

            divText.style.fontFamily = fs.FontName;
            if (fs.FontStyle == "Regular") {
                divText.style.fontStyle = "normal";
                divText.style.fontWeight = "normal";
            }
            else if (fs.FontStyle == "Italic") {
                divText.style.fontStyle = "italic";
                divText.style.fontWeight = "normal";
            }
            else if (fs.FontStyle == "Bold" ) {
                divText.style.fontStyle = "normal";
                divText.style.fontWeight = "bold";

            }
            else if (fs.FontStyle == "Bold Italic") {
                divText.style.fontStyle = "italic";
                divText.style.fontWeight= "bold";
            }
            divText.style.fontSize=fs.FontSize;

            if (fs.Strikeout && fs.Underline)
                divText.style.textDecoration = "underline line-through"
            else if (fs.Strikeout) 
                divText.style.textDecoration = "line-through";
            else if (fs.Underline)
                divText.style.textDecoration = "underline";
            else
                divText.style.textDecoration = "none";

            divText.style.color=fs.ColorName;
            var divRect = divText.getBoundingClientRect();
            divText.setAttribute("data-dropwidth", parseInt(divRect.width,10));
            divText.setAttribute("data-dropheight", parseInt(divRect.height,10));

            // display settings
            divText.style.backgroundColor = backColor;
            divText.style.borderColor = borderColor;
            divText.style.borderStyle = borderStyle.toLowerCase();
            divText.style.borderWidth = borderWidth + "px";

            divText.setAttribute("data-backcolor",backColor);
            divText.setAttribute("data-bordercolor",borderColor);
            divText.setAttribute("data-borderstyle",borderStyle);
            divText.setAttribute("data-borderwidth",borderWidth);

        }
        else if (CurrentField.id == "DetailSettings") {
            fsDetail=fs;
            dsDetail = ds;
            applySettings(CurrentField.id);
            applyDisplaySettings(CurrentField.id);
            showDetails = true;
        }
        else if (CurrentField.id == "CaptionSettings") {
            fsCaption=fs;
            dsCaption = ds;
            applySettings(CurrentField.id);
            applyDisplaySettings(CurrentField.id);
            showDetails = true;
        }
        else if (CurrentField.id == "HeaderFieldSettings" ) {
            fsHeaderFieldSettings = fs;
            dsHeaderFieldSettings =ds;
            applyHeaderFooterFieldSettings(CurrentField.id);
            showSection("divHeader");
        }
        else if (CurrentField.id == "FooterFieldSettings" ) {
            fsFooterFieldSettings = fs;
            dsFooterFieldSettings =ds;
            applyHeaderFooterFieldSettings(CurrentField.id);
            showSection("divFooter");
        }
        CurrentField = null;
    }
    if (showDetails)
        showSection("divDrop");

    closePopup(popup);
}

function closePopup(popup) {
    if (popup!= void 0) {
        var divPopupDisplay = document.getElementById(popup);
        if (popup == "divImageFieldSettingsBackground") {
            removeRadioButtonEvents();
            removeOtherEvents();
        }
        divPopupDisplay.style.display = "none";

    }
}

function clearListbox(lb) {
    if (lb!=void 0 && lb != undefined) {
        for (var i=lb.options.length-1; i>=0; i--) {
            lb.remove(i);
        }
    }
}

function loadFontList(lb) {
    clearListbox(lb)
    for (var i=0; i<availableFonts.length; i++) {
        var opt=document.createElement("OPTION");
        opt.value=availableFonts[i];
        opt.text=availableFonts[i]
        lb.add(opt);
    }

}

function showSample() {
    var lstFonts = document.getElementById("lstFonts");
        var lstFontStyles = document.getElementById("lstFontStyles");
    var lstFontSize = document.getElementById("lstFontSize");
    var lstBorderStyles = document.getElementById("lstBorderStyles");
    var ckStrikeout = document.getElementById("ckStrikeout");
    var ckUnderline = document.getElementById("ckUnderline");
    var txtBorderWidth = document.getElementById("txtBorderWidth");
    var divColorName = document.getElementById("divColorName");
    var divBackColorName = document.getElementById("divBackColorName");
    var divBorderColorName = document.getElementById("divBorderColorName");
    var divSampleText = document.getElementById("divSampleText");
    var divSampleDisplay = document.getElementById("divSampleDisplay");

    var fs = new FontSettings();
    var fontName = lstFonts.value;
    var fontStyle = lstFontStyles.value;
    var fontSize = lstFontSize.value;
    var strikeout = ckStrikeout.checked;
    var underline=ckUnderline.checked;
    var fontColor=divColorName.textContent;

    if (fontName != void 0 && fontName != "")
        fs.FontName=fontName;
    if (fontStyle != void 0 && fontStyle != "")
        fs.FontStyle=fontStyle;
    if (fontSize != void 0 && fontSize !="")
        fs.FontSize = fontSize + "pt";
    fs.Strikeout=strikeout;
    fs.Underline = underline;

    if (!fontColor.startsWith("#")) {
        fs.ColorName = fontColor;
        fs.ColorHex = toHex(fontColor);
    }
    else {
        fs.ColorName = fontColor;
        fs.ColorHex = fontColor;
    }
    divSampleText.style.fontFamily = fs.FontName;
    if (fs.FontStyle == "Regular") {
        divSampleText.style.fontStyle = "normal";
        divSampleText.style.fontWeight = "normal";
    }
    else if (fs.FontStyle == "Italic") {
        divSampleText.style.fontStyle = "italic";
        divSampleText.style.fontWeight = "normal";
    }
    else if (fs.FontStyle == "Bold" ) {
        divSampleText.style.fontStyle = "normal";
        divSampleText.style.fontWeight = "bold";

    }
    else if (fs.FontStyle == "Bold Italic") {
        divSampleText.style.fontStyle = "italic";
        divSampleText.style.fontWeight= "bold";
    }
    divSampleText.style.fontSize=fs.FontSize;

    if (fs.Strikeout && fs.Underline)
        divSampleText.style.textDecoration = "underline line-through"
    else if (fs.Strikeout)
        divSampleText.style.textDecoration = "line-through";
    else if (fs.Underline)
        divSampleText.style.textDecoration = "underline";
    else
        divSampleText.style.textDecoration = "none";

    divSampleText.style.color=fs.ColorHex;
    
    divSampleDisplay.style.backgroundColor = divBackColorName.textContent;
    divSampleDisplay.style.borderStyle = lstBorderStyles.value.toLowerCase();
    divSampleDisplay.style.borderColor = divBorderColorName.textContent;
    divSampleDisplay.style.borderWidth = txtBorderWidth.value + "px";
}

function findListValue(lb, val) {
    if (val.includes('"'))
        val = val.replaceAll('"', '');

    if (lb != void 0 && lb != undefined) {
        for (var i = 0; i < lb.length; i++) {
            var opt = lb.options[i];
            if (opt.value.toUpperCase() == val.toUpperCase()) {
                lb.options[i].selected = true;
                return i;
                break;
            }
        }
    }

    return -1
}
function findListText(lb,txt) {

    if (txt.includes('"'))
        txt=txt.replaceAll('"','');

    if (lb != void 0 && lb != undefined) {
        for (var i=0; i<lb.length; i++) {
            var opt = lb.options[i];
            if (opt.text.toUpperCase() == txt.toUpperCase()) {
                lb.options[i].selected=true;
                return i;
                break;
            }
        }
    }
    return -1;
}
//function onFontInputChanged(e) {
//    var txtFont = e.target;
//    var val = txtFont.value;
//    var lstFonts=document.getElementById("lstFonts");
//    var oldVal = lstFonts.value;
//    if (val != void 0 && val != "") {
//        var selectedIndex = findListText(lstFonts,val);
//        if (selectedIndex > -1) {
//            txtFont.value = lstFonts.value;
//            showSample();
//        }
//        else
//            txtFont.value = oldVal;
//    }
//    else
//        txtFont.value= oldVal;
//    //txtFont.focus();
//    txtFont.select();
//}

function onBorderStyleListChanged(e) {
    var borderStyleList = e.target;

    if (borderStyleList.id == "lstBorderStyles") {
        showSample();
    }
    else if (borderStyleList.id == "lstImageFieldBorderStyles") {

        fieldSettings.BorderStyle = borderStyleList.value;

        showSampleImage();
    }
    
}

function onBorderWidthInputChanged(e) {
    var borderWidthInput = e.target;

    if (borderWidthInput.id == "txtBorderWidth") {
        showSample();
    }
    else if (borderWidthInput.id == "txtImageFieldBorderWidth") {

        fieldSettings.BorderWidth = borderWidthInput.value;
        showSampleImage();
    }
   
}
function onFontListChanged(e) {
    showSample();
    //var lstFont = e.target;
    //var val = lstFont.value;
    //var txtFont = document.getElementById("txtFont");
    //if (val != void 0 && val != "") {
    //    txtFont.value=val;
    //    showSample();
    //    txtFont.select();
    //}
}

function onStyleListChanged(e) {
    showSample();
}

//function onSizeInputChanged(e) {

//}

function onSizeListChanged(e) {
    showSample();
}
function onColorFocus(e) {
    e.currentTarget.removeEventListener("focus", onColorFocus);
    e.currentTarget.addEventListener("blur",hideList);       
}

function clearSelections(tblColorList) {
    //var tblColorList = document.getElementById("tblColorList");
    var tblid =tblColorList.id;
    var endingID = "Color"
    if (tblid.indexOf("BackColor") > -1)
        endingID = "BackColor"
    else if (tblid.indexOf("BorderColor") > -1)
        endingID = "BorderColor"


    if (tblColorList != void 0) {
        for (var i=0; i<tblColorList.rows.length; i++) {
            var tr = tblColorList.rows[i];
            var cl = tr.dataset.colorname;
            //var id = "divColorText_" + cl;
            var id = "div" + endingID + "Text_" + cl;
            var div = document.getElementById(id);
            if (div.dataset.selected) {
                div.style.border = "none";
                div.dataset.selected=false;
                tr.dataset.selected=false;
                id = "div" + endingID + "_" + cl;
                div = document.getElementById(id);
                div.dataset.selected=false;
            }
        }
    }
    //for (var i=0; i<tblColorList.rows.length; i++) {
    //    var tr = tblColorList.rows[i];
    //    var cl = tr.dataset.colorname;
    //    var id = "divColorText_" + cl;
    //    var div = document.getElementById(id);
    //    if (div.dataset.selected) {
    //        div.style.border = "none";
    //        div.dataset.selected=false;
    //        tr.dataset.selected=false;
    //        div = document.getElementById("divColor_" + cl)
    //        div.dataset.selected=false;
    //    }
    //}
    tblColorList.dataset.selectedcolor="";
}
function selectColor(color,tblColorList) {
    //var tblColorList = document.getElementById("tblColorList");
    if (tblColorList != void 0) {
        var tblid =tblColorList.id;
        var endingID = "Color"
        if (tblid.indexOf("BackColor") > -1)
            endingID = "BackColor"
        else if (tblid.indexOf("BorderColor") > -1)
            endingID = "BorderColor"

        for (var i=0; i<tblColorList.rows.length; i++) {
            var tr = tblColorList.rows[i];
            var cl = tr.dataset.colorname;
            if (cl.toLowerCase() == color.toLowerCase()) {
                //var id = "divColorText_" + cl;
                var id = "div" + endingID + "Text_" + cl;
                var div = document.getElementById(id);
                div.style.borderStyle = "dotted";
                div.style.borderWidth = "1px";
                div.style.borderColor = "black";
                div.dataset.selected=true;
                tr.dataset.selected=true;
                id = "div" + endingID + "_" + cl;
                div = document.getElementById(id);
                div.dataset.selected=true;
                tblColorList.dataset.selectedcolor=cl;
                break;
            }
        }
    }
}

function onChooseColorClicked(e) {
    var divChooseColor = e.currentTarget;
    var divColorList; // = document.getElementById("divColorList");
    var clr; // = document.getElementById("divColorName").textContent;
    var tblColorList;
    var endingID;
    var divMain = document.getElementById("divMain");
    var rctMain = divMain.getBoundingClientRect();

    e.stopPropagation();

    if (divChooseColor != void 0) {
        switch (divChooseColor.id) {
            case "divChooseColor":
                divColorList = document.getElementById("divColorList");
                tblColorList = document.getElementById("tblColorList");
                clr = document.getElementById("divColorName").textContent;
                endingID = "Color"
                break;
            case "divChooseFieldTextColor":
                divColorList = document.getElementById("divColorList");
                tblColorList = document.getElementById("tblColorList");
                clr = document.getElementById("divFieldTextColorName").textContent;
                endingID = "Color"
                break;
            case "divChooseBackColor":
                divColorList = document.getElementById("divBackColorList");
                tblColorList = document.getElementById("tblBackColorList");
                clr = document.getElementById("divBackColorName").textContent;
                endingID = "BackColor";
                break;
            case "divHeaderFooterChooseBackColor":
                divColorList = document.getElementById("divBackColorList");
                tblColorList = document.getElementById("tblBackColorList");
                clr = document.getElementById("divHeaderFooterBackColorName").textContent;
                endingID = "BackColor";
                break;
            case "divChooseBorderColor":
                divColorList = document.getElementById("divBorderColorList");
                tblColorList = document.getElementById("tblBorderColorList");
                clr = document.getElementById("divBorderColorName").textContent;
                endingID = "BorderColor"
                break;
            case "divHeaderFooterChooseBorderColor":
                divColorList = document.getElementById("divBorderColorList");
                tblColorList = document.getElementById("tblBorderColorList");
                clr = document.getElementById("divHeaderFooterBorderColorName").textContent;
                endingID = "BorderColor"
                break;
            case "divImageFieldChooseBorderColor":
                divColorList = document.getElementById("divBorderColorList");
                tblColorList = document.getElementById("tblBorderColorList");
                clr = document.getElementById("divImageFieldBorderColorName").textContent;
                endingID = "BorderColor"

                break;
        }
        tblColorList.setAttribute("tag",divChooseColor.id);
        clearSelections(tblColorList);

    //if (divChooseColor != void 0) {

        var rect = divChooseColor.getBoundingClientRect();
        var topColorList;
        var calcBottom = rect.bottom + 245;

        //var zindex = divColorList.style.zIndex;
        //divColorList.style.zIndex = "2147483700";
        //zindex = divColorList.style.zIndex;

        if (rect.x != void 0) {

            var left = parseInt(rect.x, 10) + "px";
            var top = parseInt(rect.bottom, 10) + "px";

            if (calcBottom > rctMain.bottom) {
                topColorList = rect.top - 245;
                top= parseInt(topColorList, 10) + "px";
            }

            if (divColorList.style.top == "") {
                divColorList.style.left=left;
                divColorList.style.top=top;
            }
            //divColorList.style.display = "";
            //var rctColorList = divColorList.getBoundingClientRect();

            //if (rctColorList.bottom > rctMain.bottom) {
            //    topColorList = rect.top - rctColorList.height;
            //    divColorList.style.top = parseInt(topColorList, 10) + "px";
            //}

            divColorList.style.display = "";

            divColorList.scrollTop=0;
            //var scrollTo = "divColor_" + clr;
            var scrollTo = "div" + endingID + "_" + clr;
            var el=document.getElementById(scrollTo);
            if (el != void 0 && el != undefined) {
                el.scrollIntoView(true);
                selectColor(clr,tblColorList);
            }

            //divColorList.style.display = "";

            divColorList.addEventListener("focus",onColorFocus);
            divColorList.focus();
        }
    }
}
function onBtnColorChanged(e) {
    var value=e.target.value;
    var id = e.target.id

    if (value != void 0 && value != undefined) {
        var divShowColor; // = document.getElementById("divShowColor");
        var divColorName; // = document.getElementById("divColorName");

        switch(id) {
            case "btnColor":
                divShowColor = document.getElementById("divShowColor");
                divColorName = document.getElementById("divColorName");
                break;
            case "btnFieldTextColor":
                divShowColor = document.getElementById("divShowFieldTextColor");
                divColorName = document.getElementById("divFieldTextColorName");
                break;
            case "btnBackColor":
                divShowColor = document.getElementById("divShowBackColor");
                divColorName = document.getElementById("divBackColorName");
                break;
            case "btnHeaderFooterBackColor":
                divShowColor = document.getElementById("divHeaderFooterShowBackColor");
                divColorName = document.getElementById("divHeaderFooterBackColorName");
                break;
            case "btnBorderColor":
                divShowColor = document.getElementById("divShowBorderColor");
                divColorName = document.getElementById("divBorderColorName");
                break;
            case "btnHeaderFooterBorderColor":
                divShowColor = document.getElementById("divHeaderFooterShowBorderColor");
                divColorName = document.getElementById("divHeaderFooterBorderColorName");
                break;
            case "btnImageFieldBorderColor":
                divShowColor = document.getElementById("divImageFieldShowBorderColor");
                divColorName = document.getElementById("divImageFieldBorderColorName");

                break;
        }
        var clr = fromHex(value);

        if (clr == void 0)
            clr=value;
        divColorName.textContent = clr;
        divShowColor.style.backgroundColor = clr;
        e.target.removeEventListener("change",onBtnColorChanged);
        if (id != "btnHeaderFooterBackColor" && id != "btnHeaderFooterBorderColor" && id != "btnFieldTextColor" && id != "btnImageFieldBorderColor")
            showSample();
        else if (id == "btnImageFieldBorderColor") {
            fieldSettings.BorderColorName = clr;
            if (clr.startsWith("#"))
                fieldSettings.BorderColorHex = clr;
            else
                fieldSettings.BorderColorHex = toHex(clr);
            showSampleImage();
        }
    }
}

function showReport(e) {
    var btnShow = e.target;

    msgAction = "Show Report";
    showMessage("Report Format has been updated. ","Report Designer - Save And Show Report",enumMessageType.Information);
}

function returnToDesigner(e) {
    var btnReturn = e.target;

    msgAction = "Return To Designer";
    showMessage("Report Format has been updated. ","Report Designer - Save And Return To Designer",enumMessageType.Information);
}

function closeDesigner(e) {
    var btnExit = e.target;

    msgAction = "Close Designer";
    showMessage("Report Format has been updated. ","Report Designer - Save And Close Designer",enumMessageType.Information);

}

function onCustomColorClick(e) {
    var btnCustomize = e.currentTarget;

    var btnColor; // = document.getElementById("btnColor");
    var divColorName; // = document.getElementById("divColorName");
    //var clr=divColorName.textContent;
    e.stopPropagation();
    switch (btnCustomize.id) {
        case "btnCustomize":
            btnColor = document.getElementById("btnColor");
            divColorName = document.getElementById("divColorName");
            break;
        case "btnBackColorCustomize":
            btnColor = document.getElementById("btnBackColor");
            divColorName = document.getElementById("divBackColorName");
            break;
        case "btnHeaderFooterBackColorCustomize":
            btnColor = document.getElementById("btnHeaderFooterBackColor");
            divColorName = document.getElementById("divHeaderFooterBackColorName");
            break;        
        case "btnBorderColorCustomize":
            btnColor = document.getElementById("btnBorderColor");
            divColorName = document.getElementById("divBorderColorName");
            break;
        case "btnHeaderFooterBorderColorCustomize":
            btnColor = document.getElementById("btnHeaderFooterBorderColor");
            divColorName = document.getElementById("divHeaderFooterBorderColorName");
            break;
        case "btnCustomizeFieldTextColor":
            btnColor = document.getElementById("btnFieldTextColor");
            divColorName = document.getElementById("divFieldTextColorName");
            break;
        case "btnImageFieldBorderColorCustomize":
            btnColor = document.getElementById("btnImageFieldBorderColor");
            divColorName = document.getElementById("divImageFieldBorderColorName");
            break;
    }

    var clr = divColorName.textContent;

    if (!clr.startsWith("#"))
        clr=toHex(clr);

    btnColor.value=clr;
    //e.stopPropagation();
    btnColor.addEventListener("change",onBtnColorChanged);
    btnColor.click();
}

function onHeaderFooterCheckboxChanged(e) {
    var ckBox = e.target;
    var divFieldTextColorName = document.getElementById("divFieldTextColorName");
    var divShowFieldTextColor = document.getElementById("divShowFieldTextColor");

    if (ckBox.id == "ckSetTextColors") {
        var divFieldTextColorEntry = document.getElementById("divFieldTextColorEntry");
        var checked = ckBox.checked;

        divFieldTextColorName.textContent = "black";
        divShowFieldTextColor.style.backgroundColor = "black";

        if (checked)
            divFieldTextColorEntry.style.display = "";
        else {
            divFieldTextColorEntry.style.display = "none";
        }
           
    }
}

function onTextEffectsChanged(e) {
    //var ckStrikeout = document.getElementById("ckStrikeout");
    //var ckUnderline = document.getElementById("ckUnderline");
    //if (e.target.id == "ckStrikeout")
    //    ckUnderline.checked=false;
    //else
    //    ckStrikeout.checked=false;

    showSample();
}
function showColorList(color) {

}
function toHexFromRGB(rgb) {
    var rgb = rgb.split("(")[1].split(")")[0]; // gets "r,g,b" string)
    rgb=rgb.split(","); // gets individual r g b values
    
    // convert indivual values to hex
    var r = parseInt(rgb[0],10).toString(16);
    var b = parseInt(rgb[1],10).toString(16);
    var g = parseInt(rgb[2],10).toString(16);

    // add leading "0" if necessary
    r = (r.length==1) ? "0" + r: r;
    b = (b.length==1) ? "0" + b: b;
    g = (g.length==1) ? "0" +g: g;

    return "#" + r + b + g;
};

function showTabularColWidthDlg() {
    var divColWidthDlgDisplay = document.getElementById("TabularWidthBackground");

    divColWidthDlgDisplay.style.display = '';
    loadColWidthSizer();
}

function addControlOutline(e) {
    var ctrl = e.target;

    if (e.target != void 0) {
        ctrl.style.outline = "thin solid black";
        
    }
}

function removeControlOutline(e) {
    var ctrl = e.target;
    if (e.target != void 0) {
        ctrl.style.outline = "none";

    }
}

function onOtherRBChanged(e) {
    var rb = e.target;
    var field = CurrentField;

    var txtImageWidth = document.getElementById("txtImageWidth");
    var txtImageHeight = document.getElementById("txtImageHeight");
    var lstSizeOptions = document.getElementById("lstSizeOptions");

    //var ckbSquare = document.getElementById("ckbSquare");
    var divImageSample = document.getElementById("divImageSample");
    var imgSample = document.getElementById("imgSample");
    var btnSelectImageFile = document.getElementById("btnSelectImageFile");

    txtImageWidth.removeEventListener("change", onSizeTextBoxChanged);
    txtImageHeight.removeEventListener("change", onSizeTextBoxChanged);
    txtImageWidth.value = "";
    txtImageHeight.value = "";
    txtImageWidth.setAttribute("disabled", "disabled");
    txtImageHeight.setAttribute("disabled", "disabled"); 

    findListValue(lstSizeOptions, "Square");
    fieldSettings.SizeOption = "Square";
    lstSizeOptions.setAttribute("disabled", "disabled"); 

    //ckbSquare.removeEventListener("change", onCkBoxSquareChanged);
    //ckbSquare.checked = true;
    //ckbSquare.setAttribute("disabled", "disabled");

    btnSelectImageFile.focus();
    setTimeout(() => {
        btnSelectImageFile.focus();
    }, 100);

    rbChosenID = rb.id;
    var dimensions= rb.id.split("rb")[1];
    var parts = dimensions.split("x");
    var width = parts[0]; //parseInt(parts[0], 10);
    var height = parts[1]; //parseInt(parts[1], 10);

    imgSample.style.width = width +"px";
    imgSample.style.height = height + "px";


        divImageSample.style.height = (parseInt(height, 10) + 10) + "px";
        divImageSample.style.width = (parseInt(width, 10) + 10) + "px";


    fieldSettings.ImageWidth = width;
    fieldSettings.ImageHeight = height;
    showSampleImage();
}

function onSizeOptionListChanged(e) {
    var lstSizeOptions = e.target;
    var txtImageWidth = document.getElementById("txtImageWidth");
    var txtImageHeight = document.getElementById("txtImageHeight");
    var imgSample = document.getElementById("imgSample");
    var divImageSample = document.getElementById("divImageSample");
    var selectedIndex = lstSizeOptions.selectedIndex;
    var optionValue;
    var aspectRatio;

    if (selectedIndex > -1) {
        optionValue = lstSizeOptions.options[selectedIndex].value

        switch (optionValue) {
            case "Square":
                txtImageHeight.value = txtImageWidth.value;
                if (txtImageWidth.value != void 0 && parseInt(txtImageWidth.value, 10) > 15) {
                    imgSample.style.width = txtImageWidth.value + "px";
                    imgSample.style.height = txtImageHeight.value; + "px";
                    divImageSample.style.height = (parseInt(txtImageHeight.value, 10) + 10) + "px";
                    divImageSample.style.width = (parseInt(txtImageWidth.value, 10) + 10) + "px";

                    fieldSettings.ImageWidth = txtImageWidth.value;
                    fieldSettings.ImageHeight = txtImageHeight.value;
                    fieldSettings.SizeOption = "Square";
                    showSampleImage();
                }
                break;
            case "KeepAspectRatio":
                aspectRatio = parseFloat(fieldSettings.AspectRatio).toFixed(2)
                if (aspectRatio != 1.0) {
                    if (txtImageWidth.value != void 0 && parseInt(txtImageWidth.value, 10) > 15) {
                        txtImageHeight.value = (parseFloat(txtImageWidth.value) / aspectRatio).toFixed().toString();
                        imgSample.style.width = txtImageWidth.value + "px";
                        imgSample.style.height = txtImageHeight.value; + "px";
                        divImageSample.style.height = (parseInt(txtImageHeight.value, 10) + 10) + "px";
                        divImageSample.style.width = (parseInt(txtImageWidth.value, 10) + 10) + "px";

                        fieldSettings.ImageWidth = txtImageWidth.value;
                        fieldSettings.ImageHeight = txtImageHeight.value;
                        fieldSettings.SizeOption = "KeepAspectRatio";

                        findListValue(lstSizeOptions, fieldSettings.SizeOption); // Size Option

                        showSampleImage();
                    }
                }
                break;
            case "FreeForm":
                break;
        }
    }
}
function onCkBoxSquareChanged(e) {
    var ckbSquare = e.target;
    var txtImageWidth = document.getElementById("txtImageWidth");
    var txtImageHeight = document.getElementById("txtImageHeight");
    var imgSample = document.getElementById("imgSample");
    var divImageSample = document.getElementById("divImageSample");

    if (ckbSquare.checked) {
        txtImageHeight.value = txtImageWidth.value;
        if (txtImageWidth.value != void 0 && parseInt(txtImageWidth.value, 10) > 15 ) {
            imgSample.style.width = txtImageWidth.value + "px";
            imgSample.style.height = txtImageHeight.value; + "px";
            divImageSample.style.height = (parseInt(txtImageHeight.value, 10) + 10) + "px";
            divImageSample.style.width = (parseInt(txtImageWidth.value, 10) + 10) + "px";

            fieldSettings.ImageWidth = txtImageWidth.value;
            fieldSettings.ImageHeight = txtImageHeight.value;
            showSampleImage();
        }
    }
}
function onSizeTextBoxChanged(e) {
    var tbox = e.target;
    var txtImageWidth = document.getElementById("txtImageWidth");
    var txtImageHeight = document.getElementById("txtImageHeight");
    //var ckbSquare = document.getElementById("ckbSquare");
    var lstSizeOptions = document.getElementById("lstSizeOptions");
    var selectedIndex = lstSizeOptions.selectedIndex;
    var optionValue = "Square";
    var btnSelectImageFile = document.getElementById("btnSelectImageFile");
    var imgSample = document.getElementById("imgSample");
    var divImageSample = document.getElementById("divImageSample");
    var bValuesSet = false;
    var ImageWidth, ImageHeight;
    var AspectRatio;

    if (selectedIndex > -1)
        optionValue = lstSizeOptions.options[selectedIndex].value;

    if (optionValue == "Square") {
        if (tbox.id == "txtImageWidth" && txtImageWidth.value != void 0 && parseInt(txtImageWidth.value, 10) > 15) {
            txtImageHeight.value = txtImageWidth.value;
            setTimeout(() => {
                btnSelectImageFile.focus();
            }, 100);
        }

        if (tbox.id == "txtImageHeight" && txtImageHeight.value != void 0 && parseInt(txtImageHeight.value, 10) > 15) {
            txtImageWidth.value = txtImageHeight.value;
            setTimeout(() => {
                btnSelectImageFile.focus();
            }, 100);
        }
    }
    else if (optionValue == "KeepAspectRatio") {
        AspectRatio = parseFloat(fieldSettings.AspectRatio)

        if (tbox.id == "txtImageWidth" && txtImageWidth.value != void 0 && parseInt(txtImageWidth.value, 10) > 15) {
            ImageHeight = parseFloat(parseFloat(txtImageWidth.value) / AspectRatio).toFixed().toString();
            txtImageHeight.value = ImageHeight;
            setTimeout(() => {
                btnSelectImageFile.focus();
            }, 100);
        }

        if (tbox.id == "txtImageHeight" && txtImageHeight.value != void 0 && parseInt(txtImageHeight.value, 10) > 15) {
            ImageWidth = parseFloat(parseFloat(txtImageHeight.value) * AspectRatio).toFixed().toString();
            txtImageWidth.value = ImageWidth;
            setTimeout(() => {
                btnSelectImageFile.focus();
            }, 100);
        }

    }

    if (txtImageWidth.value != void 0 && parseInt(txtImageWidth.value, 10) > 15 && txtImageHeight.value != void 0 && parseInt(txtImageHeight.value, 10) > 15) {
        imgSample.style.width = txtImageWidth.value + "px";
        imgSample.style.height = txtImageHeight.value; + "px";
        divImageSample.style.height = (parseInt(txtImageHeight.value, 10) + 10) + "px";
        divImageSample.style.width = (parseInt(txtImageWidth.value, 10) + 10) + "px";

        fieldSettings.ImageWidth = txtImageWidth.value;
        fieldSettings.ImageHeight = txtImageHeight.value
        showSampleImage();
    }
    else if (txtImageWidth.value == void 0 || parseInt(txtImageWidth.value, 10) < 16) {
        setTimeout(() => {
            txtImageWidth.focus();
        }, 100);
        showMessage("Image Width Must Be Greater Than 15.", "Incorrect Image Width", enumMessageType.Error);
    }
    else if (txtImageHeight.value == void 0 || parseInt(txtImageHeight.value, 10) < 16) {
        setTimeout(() => {
            txtImageHeight.focus();
        }, 100);
        showMessage("Image Height Must Be Greater Than 15.", "Incorrect Image Height", enumMessageType.Error);
    }

}
function onCustomRBChanged(e) {
    var rbCustom = e.target;
    /*var rbCustom = document.getElementById("rbCustom");*/
    //var field = CurrentField;

    var txtImageWidth = document.getElementById("txtImageWidth");
    var txtImageHeight = document.getElementById("txtImageHeight");
    //var ckbSquare = document.getElementById("ckbSquare");
    var divImageSample = document.getElementById("divImageSample");
    var imgSample = document.getElementById("imgSample");
    var lstSizeOptions = document.getElementById("lstSizeOptions");
    var ImageWidth = fieldSettings.ImageWidth;   //field.dataset.imagewidth;
    var ImageHeight = fieldSettings.ImageHeight;  // field.dataset.imageheight;

    
    txtImageWidth.removeAttribute("disabled");
    txtImageHeight.removeAttribute("disabled");
    lstSizeOptions.removeAttribute("disabled");

    findListValue(lstSizeOptions, fieldSettings.SizeOption); // Size Option

    //ckbSquare.removeAttribute("disabled");

    //if (ImageWidth != ImageHeight)
    //    ckbSquare.checked = false;
    //else
    //    ckbSquare.checked = true;

    txtImageWidth.value = ImageWidth;
    txtImageHeight.value = ImageHeight;

    imgSample.style.width =ImageWidth + "px";
    imgSample.style.height = ImageHeight + "px";


    divImageSample.style.height = (parseInt(ImageHeight, 10) + 10) + "px";
    divImageSample.style.width = (parseInt(ImageWidth, 10) + 10) + "px";
    showSampleImage();
    setTimeout(() => {
        txtImageWidth.focus();
    }, 100);
    txtImageWidth.addEventListener("change", onSizeTextBoxChanged);
    txtImageHeight.addEventListener("change", onSizeTextBoxChanged);
    lstSizeOptions.addEventListener("change", onSizeOptionListChanged);
    //ckbSquare.addEventListener("change", onCkBoxSquareChanged);
    rbChosenID = rbCustom.id;
}

function removeRadioButtonEvents() {
    var radioButtons = document.getElementsByName("size");
    for (var i = 0; i < radioButtons.length; i++) {
        var rb = radioButtons[i];
        if (rb.id != "rbCustom") {
            rb.removeEventListener("change", onOtherRBChanged);
        }
        else {
            rb.removeEventListener("change", onCustomRBChanged);
        }
    }

}

function removeOtherEvents() {
    var lstImageFieldBorderStyles = document.getElementById("lstImageFieldBorderStyles");
    var divImageFieldChooseBorderColor = document.getElementById("divImageFieldChooseBorderColor");
    var  btnImageFieldBorderColorCustomize = document.getElementById("btnImageFieldBorderColorCustomize");
    var txtImageFieldBorderWidth = document.getElementById("txtImageFieldBorderWidth");

    lstImageFieldBorderStyles.removeEventListener("focus", addControlOutline);
    lstImageFieldBorderStyles.removeEventListener("blur", removeControlOutline);
    lstImageFieldBorderStyles.removeEventListener("change", onBorderStyleListChanged);

    divImageFieldChooseBorderColor.removeEventListener("focus", addControlOutline);
    divImageFieldChooseBorderColor.removeEventListener("blur", removeControlOutline);
    divImageFieldChooseBorderColor.removeEventListener("click", onChooseColorClicked)

    btnImageFieldBorderColorCustomize.removeEventListener("click", onCustomColorClick)
    txtImageFieldBorderWidth.removeEventListener("change", onBorderWidthInputChanged);


}
function setRadioButtonEvents() {
    var radioButtons = document.getElementsByName("size");
    for (var i = 0; i < radioButtons.length; i++) {
        var rb = radioButtons[i];
        if (rb.id != "rbCustom") {
            rb.addEventListener("change", onOtherRBChanged);
        }
        else {
            rb.addEventListener("change", onCustomRBChanged);
        }
    }
}

var rbChosenID;
var fieldSettings;
function showImageSettingsDlg(div) {

    var divImageSettingsDisplay = document.getElementById("divImageFieldSettingsBackground");
    var rbCustom = document.getElementById("rbCustom");
    var lstImageFieldBorderStyles = document.getElementById("lstImageFieldBorderStyles");
    var divImageFieldChooseBorderColor = document.getElementById("divImageFieldChooseBorderColor");
    var divImageFieldBorderColorName = document.getElementById("divImageFieldBorderColorName");
    var divImageFieldShowBorderColor = document.getElementById("divImageFieldShowBorderColor");
    var btnImageFieldBorderColorCustomize = document.getElementById("btnImageFieldBorderColorCustomize");
    var txtImageFieldBorderWidth = document.getElementById("txtImageFieldBorderWidth");
    var imgSample = document.getElementById("imgSample");
    var lstSizeOptions = document.getElementById("lstSizeOptions");
    var imageWidth = div.dataset.imagewidth;
    var imageHeight = div.dataset.imageheight;
    var imageData = div.dataset.imagedata;
    var targetName = imageWidth + "x" + imageHeight;
    var rbName = "rb" + targetName;
    var rbNameEl;
    var bMatch = false
    var ifs = new ImageFieldSettings;

    ifs.ImageWidth = imageWidth;
    ifs.ImageHeight = imageHeight;
    ifs.AspectRatio = div.dataset.aspectratio;
    ifs.SizeOption = div.dataset.sizeoption;
    ifs.ImagePath = div.dataset.imagepath;
    ifs.ImageData = imageData;
    ifs.BorderColorName = div.dataset.bordercolor;
    if (div.dataset.bordercolor.startsWith("#"))
        ifs.BorderColorHex = div.dataset.bordercolor;
    else
        ifs.BorderColorHex = toHex(div.dataset.bordercolor);
    ifs.BorderStyle = div.dataset.borderstyle;
    ifs.BorderWidth = div.dataset.borderwidth;

    //copy ifs to fieldSettings
    fieldSettings = JSON.parse(JSON.stringify(ifs));

    if (imageData != "") {
        imgSample.src = imageData;
        showSampleImage();
    }
    else if (fieldSettings.ImagePath != "" && fieldSettings.ImagePath.includes("/")) {
        imgSample.src = fieldSettings.ImagePath
        showSampleImage();
    }
    //findListValue(lstSizeOptions, fieldSettings.SizeOption); // Size Option
    findListText(lstImageFieldBorderStyles,fieldSettings.BorderStyle)  // Border Style
    txtImageFieldBorderWidth.value = fieldSettings.BorderWidth;     // Border Width

    rbChosenID = null;
    
    setRadioButtonEvents();

    lstImageFieldBorderStyles.addEventListener("focus", addControlOutline);
    lstImageFieldBorderStyles.addEventListener("blur", removeControlOutline);
    lstImageFieldBorderStyles.addEventListener("change", onBorderStyleListChanged);

    divImageFieldChooseBorderColor.addEventListener("focus", addControlOutline);
    divImageFieldChooseBorderColor.addEventListener("blur", removeControlOutline);
    divImageFieldChooseBorderColor.addEventListener("click", onChooseColorClicked)
    btnImageFieldBorderColorCustomize.addEventListener("click", onCustomColorClick)

    divImageFieldBorderColorName.textContent = fieldSettings.BorderColorName;
    divImageFieldShowBorderColor.style.backgroundColor = fieldSettings.BorderColorName;

    txtImageFieldBorderWidth.addEventListener("change", onBorderWidthInputChanged);

    divImageSettingsDisplay.style.display = "";

    var radioButtons = document.getElementsByName("size");

    // check appropriate image size radiobutton
    for (var i = 0; i < radioButtons.length; i++) {
        var rb = radioButtons[i];
        if (rb.id == rbName) {
             rbNameEl = document.getElementById(rbName)
            //rbNameEl.checked = true;
            //rbNameEl.dispatchEvent(new Event('change'));
             /* rbChosenID = rbName;*/
              bMatch = true;
              break;
        }
    }

    if (!bMatch) {
        rbCustom.checked = true;
        rbCustom.dispatchEvent(new Event('change'));
        rbChosenID = "rbCustom";
    }
    else {
        rbNameEl.checked = true;
        rbNameEl.dispatchEvent(new Event('change'));
        rbChosenID = rbName;
    }


    
}
function showHeaderFooterDlg(mnuItem) {
    //var btnSettings = mnuItem;
    var divHdrFtrDisplay = document.getElementById("divHeaderFooterDlgBackground");
    var lblHdrFtrHeading = document.getElementById("lblHeaderFooterDlgHeading");
    var txtHdrFtrHeight = document.getElementById("txtHeaderFooterHeight");
   
    var divHdrFtrShowBackColor = document.getElementById("divHeaderFooterShowBackColor");
    var divHdrFtrBackColorName = document.getElementById("divHeaderFooterBackColorName");
    var divHdrFtrChooseBackColor = document.getElementById("divHeaderFooterChooseBackColor");
    var divHdrFtrShowBorderColor = document.getElementById("divHeaderFooterShowBorderColor");
    var divHdrFtrBorderColorName = document.getElementById("divHeaderFooterBorderColorName");
    var divHdrFtrChooseBorderColor = document.getElementById("divHeaderFooterChooseBorderColor");
    var divShowFieldTextColor = document.getElementById("divShowFieldTextColor");
    var divFieldTextColorName = document.getElementById("divFieldTextColorName");
    var divChooseFieldTextColor = document.getElementById("divChooseFieldTextColor");
    var divFieldTextColorEntry = document.getElementById("divFieldTextColorEntry");
    var btnFieldTextColor = document.getElementById("btnFieldTextColor");
    var btnCustomizeFieldTextColor = document.getElementById("btnCustomizeFieldTextColor");
    var btnHdrFtrBackColor = document.getElementById("btnHeaderFooterBackColor");
    var btnHdrFtrBackColorCustomize = document.getElementById("btnHeaderFooterBackColorCustomize");
    var btnHdrFtrBorderColor = document.getElementById("btnHeaderFooterBorderColor");
    var btnHdrFtrBorderColorCustomize = document.getElementById("btnHeaderFooterBorderColorCustomize");

    var ckHeaderFooterFillFields = document.getElementById("ckHeaderFooterFillFields");
    var ckRemoveBorders = document.getElementById("ckRemoveBorders");
    var ckSetTextColors = document.getElementById("ckSetTextColors");

    var lstHdrFtrBorderStyles = document.getElementById("lstHeaderFooterBorderStyles");
    var txtHdrFtrBorderWidth = document.getElementById("txtHeaderFooterBorderWidth");

    var title;
    var ds = new HeaderFooterSettings();

    if (mnuItem.id == "HeaderSettings") {
        ds = dsHeader;
        ds.Title="Header Settings";
    }
    else {
        ds=dsFooter;
        ds.Title="Footer Settings";
    }
    title=ds.Title

    lblHdrFtrHeading.textContent = title;
    findListText(lstHdrFtrBorderStyles,ds.BorderStyle);

    divHdrFtrBackColorName.textContent = ds.BackColorName;
    divHdrFtrShowBackColor.style.backgroundColor = ds.BackColorName;
    divHdrFtrBorderColorName.textContent = ds.BorderColorName;
    divHdrFtrShowBorderColor.style.backgroundColor = ds.BorderColorName;
    divFieldTextColorName.textContent = ds.FieldSettings.TextColorName;
    divShowFieldTextColor.style.backgroundColor = ds.FieldSettings.TextColorName;

    txtHdrFtrHeight.value = ds.Height;
    txtHdrFtrBorderWidth.value = ds.BorderWidth;

    divHdrFtrChooseBackColor.addEventListener("click",onChooseColorClicked);
    divHdrFtrChooseBorderColor.addEventListener("click",onChooseColorClicked);
    divChooseFieldTextColor.addEventListener("click",onChooseColorClicked);
    btnHdrFtrBackColorCustomize.addEventListener("click",onCustomColorClick);
    btnHdrFtrBorderColorCustomize.addEventListener("click",onCustomColorClick);
    btnCustomizeFieldTextColor.addEventListener("click",onCustomColorClick);
    
    divHdrFtrDisplay.style.display="";

    ckHeaderFooterFillFields.checked = ds.FieldSettings.UseBackColor;
    ckRemoveBorders.checked = ds.FieldSettings.RemoveBorders;
    ckSetTextColors.checked = ds.FieldSettings.UseTextColor;

    ckSetTextColors.addEventListener("change",onHeaderFooterCheckboxChanged);

    if (ds.FieldSettings.UseTextColor)
        divFieldTextColorEntry.style.display = "";
    else
        divFieldTextColorEntry.style.display = "none";

    txtHdrFtrHeight.focus();
}
function showFontDlg(div) {

    var divFontDlgDisplay = document.getElementById("divFontDlgBackground");
    var lblFontDlgHeading = document.getElementById("lblFontDlgHeading");
    var lstFonts = document.getElementById("lstFonts");
    var lstFontStyles = document.getElementById("lstFontStyles");
    var lstFontSize = document.getElementById("lstFontSize");
    var ckStrikeout = document.getElementById("ckStrikeout");
    var ckUnderline = document.getElementById("ckUnderline");
    var divShowColor = document.getElementById("divShowColor");
    var divColorName = document.getElementById("divColorName");
    var divChooseColor = document.getElementById("divChooseColor");
    var divShowBackColor = document.getElementById("divShowBackColor");
    var divBackColorName = document.getElementById("divBackColorName");
    var divChooseBackColor = document.getElementById("divChooseBackColor");
    var divShowBorderColor = document.getElementById("divShowBorderColor");
    var divBorderColorName = document.getElementById("divBorderColorName");
    var divChooseBorderColor = document.getElementById("divChooseBorderColor");

    var btnColor = document.getElementById("btnColor");
    var btnCustomize = document.getElementById("btnCustomize");
    var btnBackColor = document.getElementById("btnBackColor");
    var btnBackColorCustomize = document.getElementById("btnBackColorCustomize");
    var btnBorderColor = document.getElementById("btnBorderColor");
    var btnBorderColorCustomize = document.getElementById("btnBorderColorCustomize");

    var lstBorderStyles = document.getElementById("lstBorderStyles");
    var txtBorderWidth = document.getElementById("txtBorderWidth");

    var id="0";
    var fs = new FontSettings();
    var ds = new DisplaySettings();
    var title;

    if (div !=void 0)  {
        id=div.id;
        if (id == "DetailSettings") {
            fs = fsDetail;
            ds = dsDetail;
            title = "Detail Settings"
        }
        else if (id == "CaptionSettings") {
            fs = fsCaption;
            ds = dsCaption;
            title = "Caption Settings"
        }
        else if (id == "HeaderFieldSettings") {
            fs = fsHeaderFieldSettings;
            dsHeaderFieldSettings.BackColorName = dsHeader.BackColorName;
            ds = dsHeaderFieldSettings;
            title = "Header Field Settings"
        }
        else if (id == "FooterFieldSettings") {
            fs = fsFooterFieldSettings;
            dsFooterFieldSettings.BackColorName = dsFooter.BackColorName;
            ds = dsFooterFieldSettings;
            title = "Footer Field Settings"
        }
        else if (id.startsWith("div")) {
            title = "Special Field Settings";
            if (id.startsWith("divLabel"))
                title = "Label Settings";
            if (id.startsWith("divGroupItem_")) {
                var group = div.innerText;
                title = "Group "+group+" Field Settings";
            }

            if (div.style.fontFamily != "")
                fs.FontName=div.style.fontFamily;
            if (div.style.fontStyle == "italic" && div.style.fontWeight == "bold") {
                fs.FontStyle="Bold Italic";
            }
            else if (div.style.fontStyle == "italic") {
                fs.FontStyle = "Italic";
            }
            else if (div.style.fontWeight == "bold") {
                fs.FontStyle = "Bold";
            } 
            if (div.style.fontSize != "")
                fs.FontSize=div.style.fontSize;

            if (div.style.textDecoration == "underline line-through") {
                fs.Underline = true;
                fs.Strikeout = true;
            }
            if (div.style.textDecoration == "underline")
                fs.Underline = true;
            if (div.style.textDecoration == "line-through") 
                fs.Strikeout = true;
            if (div.style.color != "") {
                if (div.style.color.startsWith("rgb(")) {
                    fs.ColorHex = toHexFromRGB(div.style.color);
                    fs.ColorName = fs.ColorHex;
                }
                else {
                    fs.ColorName=div.style.color;
                    fs.ColorHex=toHex(fs.ColorName);
                }
            }
            ds=setDisplaySettings(div);
        }
        lblFontDlgHeading.textContent = title;
    }
    var clr = fs.ColorHex
    if (clr == void 0 || clr == undefined)
        clr="#000000";

     btnColor.value=clr;
     btnBackColor.value=ds.BackColorHex;
     btnBorderColor.value = ds.BorderColorHex;

    loadFontList(lstFonts);
    //txtFont.value=fs.FontName;
    findListText(lstBorderStyles,ds.BorderStyle);
    findListText(lstFonts,fs.FontName);
    findListText(lstFontStyles,fs.FontStyle);
    //txtFontStyle.value=fs.FontStyle;
    var size = parseInt(fs.FontSize,10);
    findListText(lstFontSize,size.toString());
    //txtFontSize.value=size.toString();
    divColorName.textContent = fs.ColorName;
    divShowColor.style.backgroundColor = fs.ColorName;
    divBackColorName.textContent = ds.BackColorName;
    divShowBackColor.style.backgroundColor = ds.BackColorName;
    divBorderColorName.textContent = ds.BorderColorName;
    divShowBorderColor.style.backgroundColor = ds.BorderColorName;
    ckStrikeout.checked = fs.Strikeout;
    ckUnderline.checked = fs.Underline;

    //txtFont.addEventListener("change",onFontInputChanged);
    lstFonts.addEventListener("change",onFontListChanged);
    lstFontStyles.addEventListener("change",onStyleListChanged);
    lstFontSize.addEventListener("change",onSizeListChanged);
    ckStrikeout.addEventListener("change",onTextEffectsChanged);
    ckUnderline.addEventListener("change",onTextEffectsChanged);

    divChooseColor.addEventListener("click",onChooseColorClicked);
    divChooseBackColor.addEventListener("click",onChooseColorClicked);
    divChooseBorderColor.addEventListener("click",onChooseColorClicked);

    btnCustomize.addEventListener("click",onCustomColorClick);
    btnBackColorCustomize.addEventListener("click",onCustomColorClick);
    btnBorderColorCustomize.addEventListener("click",onCustomColorClick);
    lstBorderStyles.addEventListener("change",onBorderStyleListChanged);
    txtBorderWidth.addEventListener("change",onBorderWidthInputChanged);

    divFontDlgDisplay.style.display="";
    showSample();
    //txtFont.focus();
    //txtFont.select();
    lstFonts.focus();
}

var msgAction = "OK";
function showMessage(msg,caption,msgType) {
    var divMsgBox = document.getElementById("divMsgBox");
    var divMsgBoxDisplay = document.getElementById("divMsgBoxBackground");
    var tdImage = document.getElementById("tdImage");
    var divMsgBoxHeading = document.getElementById("divMsgBoxHeading");
    var divMsgBoxMessage = document.getElementById("divMsgBoxMessage");
    var lblCaption = document.getElementById("lblCaption");
    var btnAction = document.getElementById("btnOK");

    if (msgAction == "OK") 
        btnAction.style.width = "80px";
    else
        btnAction.style.width = "auto";
    btnAction.value = msgAction;
    divMsgBoxDisplay.style.display = "";

    // put message box in middle of screen
    divMsgBox.style.left = "50%";
    divMsgBox.style.top = "50%";
    divMsgBox.style.transform = "translate(-50%,-50%)";

    if (msgType == void 0) msgType = enumMessageType.None;

    if (caption != void 0 && caption !="") lblCaption.textContent = caption;
    if (msg != void 0 && msg != "") divMsgBoxMessage.innerText = msg;
    if (msgType != void 0) {
        var imgError = document.getElementById("imgError");
        var imgWarning = document.getElementById("imgWarning");
        var imgInfo = document.getElementById("imgInfo");

        switch (msgType) {
            case enumMessageType.None:
                tdImage.style.display = "none"
                break;
            case enumMessageType.Error:
                imgWarning.style.display = "none";
                imgInfo.style.display = "none";
                break;
            case enumMessageType.Warning:
                imgError.style.display = "none";
                imgInfo.style.display = "none";
                break;
            case enumMessageType.Information:
                imgWarning.style.display = "none";
                imgError.style.display = "none";
                break;
        }
        btnAction.focus();
    }
}
function closeMessageBoxX(e) {
    if (msgAction != "OK") {
        var action = msgAction;
        msgAction = "OK";
        closePopup("divMsgBoxBackground");
        switch(action)  {
            case "Show Report":
            case  "Return To Designer":
            case "Close Designer":
                //closePopup("divMsgBoxBackground");
                showSpinner();
                __doPostBack("MsgBoxAction","Return To Designer");
                break;
        }
    }
    else
        closePopup("divMsgBoxBackground");
}

function closeMessageBox(e) {
    if (msgAction != "OK")  {
        var action = msgAction;
        msgAction = "OK";
        closePopup("divMsgBoxBackground");
        showSpinner();
        __doPostBack("MsgBoxAction",action);
    }
   else
    closePopup("divMsgBoxBackground");
}

function getSpecialFieldCount(itemID) {
    var nHighest = 0;
    var n;
    var name = "div" + itemID +"_"
    var divHeader = document.getElementById("divHeader");
    var divFooter = document.getElementById("divFooter");
    var div;

    for (var i = 0; i < divHeader.children.length; i++) {
        div = divHeader.children[i];
        if (div.id.startsWith(name)) {
            var parts = div.id.split("_");
            n = parseInt(parts[1], 10);
            if (n > nHighest) nHighest = n;
        }
    };
    for (var i = 0; i < divFooter.children.length; i++) {
        div = divFooter.children[i];
        if (div.id.startsWith(name)) {
            var parts = div.id.split("_");
            n = parseInt(parts[1], 10);
            if (n > nHighest) nHighest = n;
        }
    };
    return nHighest
}
function getLabelCount() {
    var nHighest = 0;
    var n;
    var divDrop = document.getElementById("divDrop");
    var divHeader = document.getElementById("divHeader");
    var divFooter = document.getElementById("divFooter");
    var div;

    for (var i = 0; i < divDrop.children.length; i++) {
        div = divDrop.children[i];
        if (div.id.startsWith("divLabel_")) {
            var parts = div.id.split("_");
            n = parseInt(parts[1], 10);
            if (n > nHighest) nHighest = n;
        }
    };
    for (var i = 0; i < divHeader.children.length; i++) {
        div = divHeader.children[i];
        if (div.id.startsWith("divLabel_")) {
            var parts = div.id.split("_");
            n = parseInt(parts[1], 10);
            if (n > nHighest) nHighest = n;
        }
    };
    for (var i = 0; i < divFooter.children.length; i++) {
        div = divFooter.children[i];
        if (div.id.startsWith("divLabel_")) {
            var parts = div.id.split("_");
            n = parseInt(parts[1], 10);
            if (n > nHighest) nHighest = n;
        }
    };
    return nHighest
}
function createSizer(currentTarget) {
    var div = currentTarget.cloneNode(true);
    var divParent = currentTarget.parentElement;
    var resize =  div.dataset.resize;

    if (resize == void 0)
        resize = "horizontal";
    currentTarget.style.display="none";


    //div.style.resize="both";
    div.style.resize=resize;
    div.id = "divSizer_" + currentTarget.id;
    div.removeAttribute("oncontextmenu");
    if (miID == "") {
        div.style.display="";
        div.style.overflow="auto";
        div.addEventListener("keydown", onSizerKeyDown);
        div.addEventListener("focus", onSizerFocus);
        div.addEventListener("blur", onSizerBlur);

        divParent.insertBefore(div, currentTarget);

        div.focus()
    }
    else {
        //if (divParent.dataset.fieldformat == "Inline") {
        //    div.style.display = "inline-block";
        //}
        //else
        //    div.style.display="table";

        div.style.display = "inline-block";
        div.style.overflow = "hidden";
        div.style.whiteSpace = "nowrap";
        div.style.textOverflow = "clipped";

        divParent.addEventListener("keydown", onSizerKeyDown);
        divParent.addEventListener("focus", onSizerFocus);
        divParent.addEventListener("blur", onSizerBlur);
        divParent.insertBefore(div, currentTarget);

        divParent.focus()
    }

}
function onSpecialFieldClick(e) {
    if (e.ctrlKey) {
        createSizer(e.currentTarget);
        e.stopPropagation();
    }
    
}

function onGroupFieldClick(e) {
    if (e.ctrlKey) {
        createSizer(e.currentTarget);
        e.stopPropagation();
    }

}

function onLabelClick(e) {
    if (!e.ctrlKey) {
    editLabel(e.currentTarget);
    }
    else {
        createSizer(e.currentTarget);
    }
    e.stopPropagation();
}

//function onSizerClick(e) {

//    e.stopPropagation();
//}

function onSizerKeyDown(e) {
    var trgt = e.currentTarget;
    var keyCode = e.keyCode;
    var divSizer = document.getElementById(trgt.id);
    var bLoseFocus = (trgt.id.startsWith("divSizer_") || miID != "");

    if (bLoseFocus) {
       if (keyCode==9 || keyCode== 27) {
          e.preventDefault();
          e.stopPropagation();
          divSizer.blur();
       }
    }
}

function onSizerFocus(e) {
    var trgt = e.currentTarget;
    var divSizer = document.getElementById(trgt.id);
    var divChildSizer = "";

   if (miID == "") {
       if (trgt.id.startsWith("divSizer_")) {
           divSizer.style.border = "1px solid darkgray";
           divSizer.style.outline = "none";
       }
   }
   else {
       divChildSizer = (miID == "ResizeCaption") ? divSizer.children[0] : divSizer.children[1];
       if (divChildSizer.id.startsWith("divSizer_")) {
           divChildSizer.style.border = "1px solid darkgray";
           divChildSizer.style.outline = "none";
       }
   }
}

function onSizerBlur(e) {
    var trgt = e.currentTarget;
    var divSizer = document.getElementById(trgt.id);
    var sizedID = trgt.id.split("divSizer_")[1];
    var divSized = document.getElementById(sizedID);
    var divParent = divSizer.parentElement;
    var divSibling;
    var sizerRect;
    var parentRect;
    var siblingRect;
    var left;
    var top;
    var width;
    var height;

    if (!trgt.id.startsWith("divSizer_") && miID !="") {

        divSizer.removeEventListener("keydown", onLabelKeydown);
        divSizer.removeEventListener("focus", onLabelFocus);
        divSizer.removeEventListener("blur", onLabelBlur);

        if (miID == "ResizeCaption") {
            divSibling = divSizer.children[2];
            divSizer = divSizer.children[0]
            
        }
        else {
            divSibling = divSizer.children[0];
            divSizer = divSizer.children[1];
        }

        //divSizer = (miID == "ResizeCaption") ? divSizer.children[0] : divSizer.children[1];
        if (divSizer.id.startsWith("divSizer_")) {
            sizedID = divSizer.id.split("divSizer_")[1];
            divSized = document.getElementById(sizedID);
            divParent = divSizer.parentElement;

            sizerRect = divSizer.getBoundingClientRect();
            parentRect = divParent.getBoundingClientRect();
            siblingRect = divSibling.getBoundingClientRect();

            width = parseInt(Math.round(parentRect.width),10);
            height = parseInt(Math.round(parentRect.height),10);

            divParent.dataset.dropwidth = width;
            divParent.dataset.dropheight = height;

            divParent.style.height = height + "px";
            divParent.style.width = width + "px"

            left = parseInt(divParent.offsetLeft + sizerRect.width,10);
            if (miID == "ResizeCaption" &&divParent.dataset.fieldformat == "Inline" ) {
                    divSibling.dataset.dropleft = left;
                    divSibling.style.left = left + "px";
            }

            left = parseInt(divParent.offsetLeft + divSizer.offsetLeft,10);
            top = parseInt(divParent.offsetTop + divSizer.offsetTop,10);
            width = parseInt(Math.round(sizerRect.width),10);
            height = parseInt(Math.round(sizerRect.height),10);

            divSized.dataset.dropleft = left;
            divSized.dataset.droptop = top;
            divSized.dataset.dropwidth = width;
            divSized.dataset.dropheight = height;

            divSized.style.left = left + "px";
            divSized.style.top = top + "px";
            divSized.style.height = height + "px";
            divSized.style.width = width + "px"

            divParent.removeChild(divSizer)
            //divSized.style.display = (divParent.dataset.fieldformat == "Inline") ? "inline-block":"table";
            divSized.style.display = "inline-block";
            divSized.style.overflow = "hidden";
            divSized.style.whiteSpace = "nowrap";
            divSized.style.textOverflow = "clipped";

        }
    }

    if (divSizer.id.startsWith("divSizer_") && miID == "") {

        divSizer.removeEventListener("keydown", onLabelKeydown);
        divSizer.removeEventListener("focus", onLabelFocus);
        divSizer.removeEventListener("blur", onLabelBlur);

        divSizer.removeEventListener("keydown", onSizerKeyDown);
        divSizer.removeEventListener("focus", onSizerFocus);
        divSizer.removeEventListener("blur", onSizerBlur);


        //sizerRect = divSizer.getBoundingClientRect();
        //left = parseInt(Math.round(sizerRect.left-divParent.parentElement.offsetLeft),10);
        //top = parseInt(Math.round(sizerRect.top)-divParent.parentElement.offsetTop,10)-1;
        //top = parseInt(sizerRect.top,10) - divParent.offsetTop - 1;
        //width = parseInt(Math.round(sizerRect.width),10);
        //height = parseInt(Math.round(sizerRect.height),10);

        left = divSizer.offsetLeft;
        top = divSizer.offsetTop;
        width = divSizer.offsetWidth;
        height = divSizer.offsetHeight;


        divSized.dataset.dropleft = left;
        divSized.dataset.droptop = top;
        divSized.dataset.dropwidth = width;
        divSized.dataset.dropheight = height;

        divSized.style.left = left + "px";
        divSized.style.top = top + "px";
        divSized.style.height = height + "px";
        divSized.style.width = width + "px"

        divParent.removeChild(divSizer)
        divSized.style.display = "inline-block";
        divSized.style.overflow = "hidden";
        divSized.style.whiteSpace = "nowrap";
        divSized.style.textOverflow = "clipped";
    }
    if (divSized.id.startsWith("divImage")) {
        var img = divSized.children[0];
        if (img != void 0) {
            var imageWidth =parseInt( img.width,10);
            var imageHeight = parseInt(img.height,10);

            divSized.dataset.imagewidth = imageWidth;
            divSized.dataset.imageheight = imageHeight;


        }
    }
}

function onLabelTBClick(e) {
    e.stopPropagation();
}

function onLabelChanged(e) {
    var txtbox = e.currentTarget;
    var divTB = txtbox.parentElement;
    var parent = divTB.parentElement;
    var divLbl = parent.children[0];
    
    divLbl.textContent = txtbox.value;
    divTB.style.display = "none";
    divLbl.style.display = "";
    var divRect = parent.getBoundingClientRect();
    parent.setAttribute("data-dropwidth", parseInt(divRect.width,10));
    parent.setAttribute("data-dropheight", parseInt(divRect.height,10));
}

function dragLabel(e) {
    var clientRect = e.currentTarget.getBoundingClientRect();
    var offsetX = parseInt(e.offsetX, 10);
    var offsetY = parseInt(e.offsetY, 10);

    e.dataTransfer.dropEffect = 'move';
    e.dataTransfer.setData("text", e.target.id + "," + offsetX + "," + offsetY);
    e.dataTransfer.setData("allow_drop","");
}

function dropOnLabel(e) {
    e.preventDefault();
    var data = e.dataTransfer.getData("text");
    var params = data.split(",");
    var dropId = params[0];
    var offsetX = params[1];
    var offsetY = params[2];
    var divToDrop = document.getElementById(dropId);;
    var trgt = e.currentTarget;
    var child = e.target
    var targId = trgt.id;
    var curX = e.offsetX - offsetX;
    var curY = e.offsetY - offsetY;

    if (trgt.id != child.id) {
        curX = child.offsetLeft + curX;
        curY = child.offsetTop + curY;
    }

    if (dropId == targId) {
        var left = parseInt(divToDrop.style.left, 10) + curX;
        var top = parseInt(divToDrop.style.top, 10) + curY
        divToDrop.setAttribute("data-dropleft", parseInt(left,10));
        divToDrop.setAttribute("data-droptop", parseInt(top,10));

        divToDrop.style.left = left + "px";
        divToDrop.style.top = top + "px";    }
    else {
        e.stopPropagation();
        return false
    }
    e.stopPropagation();

}

function dragSpecialField(e) {
    var clientRect = e.currentTarget.getBoundingClientRect();
    var offsetX = parseInt(e.offsetX, 10);
    var offsetY = parseInt(e.offsetY, 10);

    e.dataTransfer.dropEffect = 'move';
    e.dataTransfer.setData("text", e.currentTarget.id + "," + offsetX + "," + offsetY);
    e.dataTransfer.setData("allow_drop","");
}

function dropOnSpecialField(e) {
    e.preventDefault();
    var data = e.dataTransfer.getData("text");
    var params = data.split(",");
    var dropId = params[0];
    var offsetX = params[1];
    var offsetY = params[2];
    var divToDrop = document.getElementById(dropId);
    var trgt = e.currentTarget;
    var targId = trgt.id;
    var curX = e.offsetX - offsetX;
    var curY = e.offsetY - offsetY;

    if (dropId == targId) {
        var left = parseInt(divToDrop.style.left, 10) + curX;
        var top = parseInt(divToDrop.style.top, 10) + curY
        divToDrop.style.left = left + "px";
        divToDrop.style.top = top + "px";
        divToDrop.setAttribute("data-dropleft", parseInt(left));
        divToDrop.setAttribute("data-droptop", parseInt(top,10));
    }
    else {
        e.stopPropagation();
        return false
    }
    e.stopPropagation();
    //return false;
}
function createSpecialField(id, caption, n) {
    var name = "div" + id;
    var divID = name + "_" + n;
    var divSpecial = document.createElement("div");
    var imgSpecial;

    divSpecial.id = divID;
    divSpecial.setAttribute("draggable", "true");
    divSpecial.setAttribute("ondragstart", "dragSpecialField(event);");
    divSpecial.setAttribute("ondrop", "dropOnSpecialField(event);");
    divSpecial.setAttribute("tabindex", "-1");
    divSpecial.setAttribute("data-resize", "both");
    divSpecial.style.display = "inline-block";
    divSpecial.style.margin = "0px";
    divSpecial.style.padding = "5px";
    //divSpecial.style.width = "auto";
    //divSpecial.style.height = "auto";
    if (id != "Image") {
        divSpecial.style.width = "auto";
        divSpecial.style.height = "auto";
        //divSpecial.textContent = caption;
        divSpecial.textContent = caption;
        divSpecial.setAttribute("oncontextmenu", "return showSpecialMenu(event);")
        doSettings(divSpecial, fsDetail);
        doDisplaySettings(divSpecial, dsDetail);
    }
    else {
        divSpecial.style.width = "74px";
        divSpecial.style.height = "74px";
        //divSpecial.style.padding = "4px";
        //divSpecial.className = "relative-middle"
        divSpecial.setAttribute("data-imagewidth", "64");
        divSpecial.setAttribute("data-imageheight", "64");
        divSpecial.setAttribute("data-sizeoption", "Square");
        divSpecial.setAttribute("data-imagepath", "");   // image file name
        divSpecial.setAttribute("data-imagedata", "");
        divSpecial.setAttribute("data-imagecaption", "Field Image " + n);
        divSpecial.setAttribute("data-aspectratio", "1.00");

        imgSpecial = document.createElement("img");
        imgSpecial.style.width = "100%";
        imgSpecial.style.height = "100%";
        imgSpecial.id = "img_" + n;
        //imgSpecial.id = "img" + caption + n;
        //imgSpecial.className = "relative-middle";
        //divSpecial.appendChild(imgSpecial);
        //divSpecial.style.width = "64px";
        //divSpecial.style.height = "64px";
        //divSpecial.style.padding = "4px";

    }

    divSpecial.addEventListener("click", onSpecialFieldClick);
    //doSettings(divSpecial,fsDetail);
    //doDisplaySettings(divSpecial, dsDetail);

    if (id == "Image") {
        divSpecial.setAttribute("oncontextmenu", "return showImageMenu(event);")
        //imgSpecial.style.width = "100%"
        //imgSpecial.style.height = "100%";
        imgSpecial.src = "Controls/Images/ReportDesignerImages/ReportDesignerImage.ico";
        imgSpecial.alt = "Field Image " + n;
        imgSpecial.title = "Field Image " + n;
        divSpecial.appendChild(imgSpecial);
        doImageSettings(divSpecial, dsImageFieldSettings);

       /* imgSpecial.src = "~/Controls/Images/ReportDesignerImages/Weather256x256.ico" */
        //divSpecial.style.width = "64px";
        //divSpecial.style.height = "64px";
        //divSpecial.style.padding = "4px";

    }

    return divSpecial;

}

function createGroupItem(itmGroup) {
    var orientation = reportView.Orientation;
    var id = itmGroup.ItemID;
    var divGroupItem = document.createElement("div");
    var caption = itmGroup.Caption;
    var itemOrder = itmGroup.ItemOrder;

    divGroupItem.id = "divGroupItem_" + id;
    divGroupItem.setAttribute("tabindex", "-1");
    divGroupItem.setAttribute("data-resize", "vertical");
    divGroupItem.style.display = "inline-block";
    divGroupItem.style.margin = "0px";
    divGroupItem.style.padding = "5px";
    divGroupItem.style.width = "8in";

    //divGroupItem.style.height = "auto"
    divGroupItem.style.height = itmGroup.Height + "px";
    divGroupItem.textContent = caption;

    if (orientation == "landscape")
        divGroupItem.style.width = "11in"

    divGroupItem.setAttribute("oncontextmenu", "return showGroupMenu(event);")

    divGroupItem.addEventListener("click", onGroupFieldClick);

    doSettings(divGroupItem, fsGroupFieldSettings);
    doDisplaySettings(divGroupItem, dsGroupFieldSettings);

    //var divText = document.createElement("div");
    //divText.id = "text_" + id;
    //divText.style.margin = "0px";
    //divText.style.padding = "0px";
    //divText.style.width = "auto";
    //divText.style.height = "auto";
    //divText.style.display = "inline-block";
    //divText.style.verticalAlign = "top";
    //divText.textContent = caption;

    //divGroupItem.appendChild(divText);
    return divGroupItem
}
function createLabel(id) {
    var divLabel = document.createElement("div");

    divLabel.id = "divLabel_" + id;
    divLabel.setAttribute("draggable", "true");
    divLabel.setAttribute("ondragstart", "dragLabel(event);");
    divLabel.setAttribute("ondrop", "dropOnLabel(event);");
    divLabel.setAttribute("tabindex", "-1");
    divLabel.setAttribute("data-resize", "both");
    divLabel.style.display = "inline-block";
    divLabel.style.margin = "0px";
    divLabel.style.padding = "5px";
    divLabel.style.width = "auto";
    divLabel.style.height = "auto";
    //divLabel.style.border = "solid 1px blue";
    //divLabel.style.fontFamily = "Tahoma";
    //divLabel.style.fontSize = "12px";
    divLabel.setAttribute("oncontextmenu", "return showLabelMenu(event);")

    divLabel.addEventListener("click", onLabelClick);

    doSettings(divLabel,fsLabel);
    doDisplaySettings(divLabel,dsLabel);

    var divText = document.createElement("div");
    divText.id = "text_" + id;
    divText.style.margin = "0px";
    divText.style.padding = "0px";
    divText.style.width = "100%";
    divText.style.height = "100%";
    //divText.style.fontFamily = "Tahoma";
    //divText.style.fontSize = 12
    divText.style.display = "";
    divText.textContent = "Label" + id;
   
    divLabel.appendChild(divText);

    var divTB = document.createElement("div");
    divTB.id = "tbText_" + id;
    //divTB.style.fontFamily = "Tahoma";
    divTB.style.margin = "0px";
    divTB.style.padding = "0px";
    divTB.style.width = "100%";
    divTB.style.height = "100%";
    divTB.style.display = "none";

    var txtbox = document.createElement("INPUT");
    txtbox.type = "text";
    txtbox.value = "Label" + id;
    txtbox.id = "txtTB_" + id;
    txtbox.style.border = "none";
    txtbox.style.outline = "none";
    txtbox.style.width = "150px";
    //txtbox.style.fontSize = 12

    txtbox.addEventListener("blur", onLabelChanged);
    txtbox.addEventListener("click", onLabelTBClick);

    divTB.appendChild(txtbox);
    divLabel.appendChild(divTB);
    return divLabel
}
function addSpecialField(menuItemID) {
    //var e = event || window.event;
    var divDrop = document.getElementById(currentSection);
    var divHeading = document.getElementById("divFieldsMenuHeading");
    var menuRect = divHeading.getBoundingClientRect();
    var offsetLeft = divDrop.offsetLeft;
    var offsetTop = divDrop.offsetTop;
    var left = parseInt(menuRect.left - offsetLeft, 10);
    var top = parseInt(menuRect.top - offsetTop, 10)
    var n = parseInt(getSpecialFieldCount(menuItemID) + 1, 10);
    
    var divField = createSpecialField(menuItemID,n);
    
    divField.setAttribute("data-itemtype",menuItemID);

    divDrop.appendChild(divField);

    var fieldno = divDrop.children.length;
    divField.setAttribute("data-fieldindex", fieldno);
    divField.setAttribute("data-textalign","Left");

    divField.style.textAlign = "left";
    divField.style.position = "absolute";
    divField.style.left = left + "px";
    divField.style.top = top + "px";

    var menuRect = divHeading.getBoundingClientRect();
    var offsetLeft = divDrop.offsetLeft;
    var offsetTop = divDrop.offsetTop;
    var left = parseInt(menuRect.left - offsetLeft, 10);
    var top = parseInt(menuRect.top - offsetTop, 10)
    var n = parseInt(getSpecialFieldCount(menuItemID) + 1, 10);
    
    var divField = createSpecialField(menuItemID,n);
    
    divField.setAttribute("data-itemtype",menuItemID);

    divDrop.appendChild(divField);

    var fieldno = divDrop.children.length;
    divField.setAttribute("data-fieldindex", fieldno);
    divField.setAttribute("data-textalign","Left");

    divField.style.textAlign = "left";
    divField.style.position = "absolute";
    divField.style.left = left + "px";
    divField.style.top = top + "px";

}
function addLabel(e) {
    var divDrop = document.getElementById(currentSection);
    var divContent = divDrop.parentNode;
    var contentRect = divContent.getBoundingClientRect();
    var divHeading = document.getElementById("divFieldsMenuHeading");
    var dropMenu = e.currentTarget;
    var menuRect = dropMenu.getBoundingClientRect();
    var divCaption;

    if (currentSection != "divDrop")
        menuRect = divHeading.getBoundingClientRect();

    var offsetLeft = divDrop.offsetLeft;
    var offsetTop = divDrop.offsetTop;
    var left = parseInt(dropMenu.dataset.x - offsetLeft, 10);
    var top = parseInt(dropMenu.dataset.y-offsetTop-1, 10);

    var n = parseInt(getLabelCount() + 1, 10);
    
    fsLabel=fsCaption;

    var divLabel = createLabel(n);

    divLabel.setAttribute("data-fieldindex", fieldno);
    divLabel.setAttribute("data-textalign","Left");
    divDrop.appendChild(divLabel);
    var fieldno = divDrop.children.length

    if (divDrop.id == "divDrop")
        fieldno = divDrop.children.length-1

    divLabel.style.position = "absolute";
    divLabel.style.left = left + "px";
    divLabel.style.top = top + "px";

    divCaption = divLabel.children[0];
    divCaption.style.textAlign = "left";

    var divRect = divLabel.getBoundingClientRect();

    divLabel.setAttribute("data-dropleft", parseInt(left,10));
    divLabel.setAttribute("data-droptop", parseInt(top,10));
    divLabel.setAttribute("data-dropwidth", parseInt(divRect.width,10));
    divLabel.setAttribute("data-dropheight", parseInt(divRect.height,10));
}
function showHelp() {
    // opens document in same browser tab or window, gets to previous page with back arrow
    //window.location.href="OnlineUserReporting.pdf#page=5";

    // opens document in new browser tab or window
    window.open("AdvancedReportDesigner.pdf#page=4","_blank");
}
function clickFileUpload() {
    var fileUpload = document.getElementById("FileRDL");
    if (fileUpload != null) {
        fileUpload.click();
    }
}

function showSampleImage() {
    var divImageSample = document.getElementById("divImageSample");
    var divChosenFile = document.getElementById("divChosenFile");
    var borderStyle = fieldSettings.BorderStyle;
    var borderColor = fieldSettings.BorderColorName;
    var borderWidth = fieldSettings.BorderWidth;

    divImageSample.style.borderWidth = borderWidth + "px";
    divImageSample.style.borderColor = borderColor;
    divImageSample.style.borderStyle = borderStyle.toLowerCase();

    if (fieldSettings.ImageData != "") {
        divImageSample.style.display = "";
        divChosenFile.textContent = fieldSettings.ImagePath;
    }
    else if (fieldSettings.ImagePath != "" && fieldSettings.ImagePath.includes("/")) {
        divImageSample.style.display = "";
        divChosenFile.textContent = "Source = " + fieldSettings.ImagePath;
    }
    else {
        divImageSample.style.display = "none";
        divChosenFile.textContent = "";
    }

         
}

function downloadImage(dataURL) {

    if (dataURL != void 0 && dataURL != '' & fieldSettings.ImagePath != '') {
        var anchor = document.createElement("a");

        anchor.setAttribute("href", dataURL);
        anchor.setAttribute("download", fieldSettings.ImagePath);
        anchor.click();
    }
}
function convertImageToPng(img) {
    var canvas = document.createElement("canvas");
    var imgSample = document.getElementById("imgSample");
    var divChosenFile = document.getElementById("divChosenFile");

    //canvas.width = img.width;
    //canvas.height = img.height;
    canvas.width = img.naturalWidth;
    canvas.height = img.naturalHeight;

    canvas.getContext("2d").drawImage(img, 0, 0);

    var pngDataURL = canvas.toDataURL("image/png");
    var newFileName;

    if (fieldSettings.ImageType  == "image/x-icon")
       newFileName = fieldSettings.ImagePath.toLowerCase().replace(".ico", ".png");
    else if (fieldSettings.ImageType == "image/svg+xml")
        newFileName = fieldSettings.ImagePath.toLowerCase().replace(".svg", ".png");
  
    fieldSettings.ImageType = "image/png";
    fieldSettings.ImageData = pngDataURL;
    fieldSettings.ImagePath = newFileName;

    divChosenFile.textContent = newFileName;
    img.src = pngDataURL;
    //downloadImage(pngDataURL);
}
function getAspectRatioOnImageLoad(e) {
    var img = e.target;

    var aspectRatio = "1.00";
    var actualAspectRatio;

    if (img != void 0) {
        var originalHeight = img.naturalHeight;
        var originalWidth = img.naturalWidth;
        var height = img.height;
        var width = img.width;
        var div = img.parentElement;

        if (originalHeight != originalWidth) {
            aspectRatio = parseFloat(parseFloat(originalWidth).toFixed() / parseFloat(originalHeight).toFixed()).toFixed(2);
            div.setAttribute("data-sizeoption", "KeepAspectRatio");
            div.setAttribute("data-aspectratio", aspectRatio.toString());

            if (width == height) {
                div.setAttribute("data-sizeoption", "Square");
            }
            else {
                actualAspectRatio = parseFloat(parseFloat(width).toFixed() / parseFloat(height).toFixed()).toFixed(1);
                if (actualAspectRatio != parseFloat(aspectRatio).toFixed(1))
                    div.setAttribute("data-sizeoption", "FreeForm");
            }
        }
        else {
            div.setAttribute("data-sizeoption", "Square");
            div.setAttribute("data-aspectratio", aspectRatio);
            if (height != width)
                div.setAttribute("data-sizeoption", "FreeForm");
        }
        img.removeEventListener("load", getAspectRatioOnImageLoad);
    }
   
}
function onReaderImageLoad(e) {
    var img = e.target;
    if (img != void 0) {
        var height = img.height;
        var width = img.width;
        var originalHeight = img.naturalHeight;
        var originalWidth = img.naturalWidth;
        var aspectRatio = "1.00";

        if (fieldSettings.ImageType == "image/svg+xml" || fieldSettings.ImageType == "image/x-icon")
              convertImageToPng(img);

        if (originalHeight != originalWidth) {
            aspectRatio = parseFloat(parseFloat(originalWidth).toFixed() / parseFloat(originalHeight).toFixed()).toFixed(2);
           /*fieldSettings.SizeOption = "KeepAspectRatio";*/
            fieldSettings.AspectRatio = aspectRatio.toString();
        }
        img.removeEventListener("load", onReaderImageLoad);
        showSampleImage();
    }
}
function onReaderLoad(e) {
    var reader = e.target;
    var imgSample = document.getElementById("imgSample");

    imgSample.addEventListener("load", onReaderImageLoad);
    fieldSettings.ImageData = reader.result;
    //downloadImage(reader.result);

    imgSample.src = reader.result;
   
    //showSampleImage();.
}

function uploadSelectedFile(e) {
    var btnUploadSelectedFile = e.target;
    var fileUpload = document.getElementById("FileRDL");
    
    if (fileUpload.files.length > 0) {
        showSpinner();
        __doPostBack("UploadSelectedFile", fileUpload.files[0].name);

    }
    else {
        var msg = "No Image File has been selected."
        showMessage(msg, "No File Selected", enumMessageType.Error);
    }
}
function getBrowser() {
    var userAgent = navigator.userAgent;

    if (userAgent.includes("Chrome") && !userAgent.includes("Edg")) {
        return "Chrome";
    }
    else if (userAgent.includes("Firefox")) {
        return "Firefox";
    }
    else if (userAgent.includes("Edg")) {
        return "Edge";
    }
}
function loadSelectedFile(e) {
    var el = e.target;
    var imgSample = document.getElementById("imgSample");

    var fileUpload = document.getElementById("FileRDL");
    var divChosenFile = document.getElementById("divChosenFile");
    var reader = new FileReader();

    var browser = getBrowser();

    var msg;

    if (fileUpload != null) {
        var chosenFile = fileUpload.files[0];
        var fileName = chosenFile.name;
        var fileType = chosenFile.type
        if (fileType.startsWith("image/")) {
            if (browser == "Firefox" && fileType == "image/svg+xml") {
                msg = fileName + " is not a valid image file - svg images are not supported for Firefox."
                showMessage(msg, "Wrong File Type Selected", enumMessageType.Warning);
            }
            else {
            divChosenFile.textContent = fileName;
            fieldSettings.ImagePath = fileName;
            fieldSettings.ImageType = fileType;
            reader.addEventListener("load", onReaderLoad);
            reader.readAsDataURL(chosenFile);
        }
        }
        else {
                msg = fileName + " is not a valid image file."
            showMessage(msg, "Wrong File Type Selected", enumMessageType.Warning);
        }
        
    }
}
