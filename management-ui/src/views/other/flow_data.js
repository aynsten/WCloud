var data = {
    "Success": true,
    "ErrorCode": null,
    "ErrorMsg": null,
    "Data": [
        [
            {
                "UID": "start",
                "Name": "开始",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "start-notification",
                        "FormConditions": [
                            {
                                "Disabled": true,
                                "Description": "2019年的数据发送通知",
                                "FieldName": "StartDate",
                                "FieldType": "System.DateTime",
                                "Operator": "same_year",
                                "Value": "2019-01-01"
                            }
                        ]
                    },
                    {
                        "NodeUID": "leader",
                        "FormConditions": null
                    },
                    {
                        "NodeUID": "super-boss",
                        "FormConditions": null
                    }
                ]
            },
            {
                "UID": "start-notification",
                "Name": "抄送通知",
                "_Position": null,
                "NextNodes": null
            }
        ],
        [
            {
                "UID": "start",
                "Name": "开始",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "start-notification",
                        "FormConditions": [
                            {
                                "Disabled": true,
                                "Description": "2019年的数据发送通知",
                                "FieldName": "StartDate",
                                "FieldType": "System.DateTime",
                                "Operator": "same_year",
                                "Value": "2019-01-01"
                            }
                        ]
                    },
                    {
                        "NodeUID": "leader",
                        "FormConditions": null
                    },
                    {
                        "NodeUID": "super-boss",
                        "FormConditions": null
                    }
                ]
            },
            {
                "UID": "leader",
                "Name": "组长",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "hr",
                        "FormConditions": [
                            {
                                "Disabled": false,
                                "Description": "请假天数",
                                "FieldName": "Days",
                                "FieldType": "System.Int32",
                                "Operator": "<=",
                                "Value": 3
                            }
                        ]
                    },
                    {
                        "NodeUID": "boss",
                        "FormConditions": [
                            {
                                "Disabled": false,
                                "Description": "请假天数",
                                "FieldName": "Days",
                                "FieldType": "System.Int32",
                                "Operator": ">",
                                "Value": 3
                            }
                        ]
                    }
                ]
            },
            {
                "UID": "boss",
                "Name": "老板",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "hr",
                        "FormConditions": null
                    }
                ]
            },
            {
                "UID": "hr",
                "Name": "hr",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "finish",
                        "FormConditions": null
                    }
                ]
            },
            {
                "UID": "finish",
                "Name": "结束",
                "_Position": null,
                "NextNodes": null
            }
        ],
        [
            {
                "UID": "start",
                "Name": "开始",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "start-notification",
                        "FormConditions": [
                            {
                                "Disabled": true,
                                "Description": "2019年的数据发送通知",
                                "FieldName": "StartDate",
                                "FieldType": "System.DateTime",
                                "Operator": "same_year",
                                "Value": "2019-01-01"
                            }
                        ]
                    },
                    {
                        "NodeUID": "leader",
                        "FormConditions": null
                    },
                    {
                        "NodeUID": "super-boss",
                        "FormConditions": null
                    }
                ]
            },
            {
                "UID": "super-boss",
                "Name": "超级大老板",
                "_Position": null,
                "NextNodes": [
                    {
                        "NodeUID": "finish",
                        "FormConditions": null
                    }
                ]
            },
            {
                "UID": "finish",
                "Name": "结束",
                "_Position": null,
                "NextNodes": null
            }
        ]
    ]
};


var flow_data = data.Data;

function rand() {
    var r = Math.random() * 1000;
    return parseInt(r);
}

var nodes = []
var lines = []

function try_push_node(a) {
    if (!nodes.some(x => x.name === a.Name)) {
        nodes.push({
            uid: a.UID,
            name: a.Name,
            x: rand(),
            y: rand()
        });
    }
}

function try_push_line(a, b) {
    if (!lines.some(x => x.source === a.Name && x.target === b.Name)) {
        lines.push({
            source: a.Name,
            target: b.Name,
            label: {
                normal: {
                    show: true
                }
            },
            lineStyle: {
                normal: { curveness: 0.2 }
            }
        });
    }
}

flow_data.forEach(x => {
    for (var i = 0; i < x.length - 1; ++i) {
        var a = x[i];
        var b = x[i + 1];

        try_push_node(a);
        try_push_node(b);

        try_push_line(a, b);
    }
});

//console.log(nodes, lines);

export { flow_data, nodes, lines }