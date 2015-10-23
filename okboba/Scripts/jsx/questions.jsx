var QuestionHeaderEdit = React.createClass({
    render: function () {
        return (
            <div className="questionHeaderEdit">
                <div className="row">
                    <div className="form-group col-md-9">
                        <input type="text" name="name" value={this.props.english} className="form-control" />
                    </div>
                    <div className="col-md-3 form-group">
                        <select className="form-control">
                            <option value="value">text</option>
                        </select>
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-9 form-group">
                        <input type="text" name="name" value={this.props.chinese} className="form-control" />
                    </div>
                    <div className="col-md-3"></div>
                </div>
            </div>
            );
    }
});

var QuestionHeader = React.createClass({
    handleClick: function () {
        alert('clicked');
    },
    
    render: function () {
        return (
            <div className="questionHeader" onClick={this.handleClick}>
                <div className="row">
                    <div className="col-sm-9">
                        {this.props.english}
                    </div>
                    <div className="col-sm-3">
                        <b>{this.props.trait}</b>
                    </div>
                </div>
                <div className="row">
                    <div className="col-sm-12">
                        {this.props.chinese}
                    </div>
                </div>
                
            </div>
            );
}
});

var ChoiceEdit = React.createClass({
    render: function () {
        return (
                <div className="row">
                    <div className="form-inline col-md-12">
                        <div className="form-group">
                            <label>Choice {this.props.i}</label>
                            <input type="text" name="name" value={this.props.english} className="form-control" />
                            <input type="text" name="name" className="form-control" value={this.props.chinese} />
                        </div>
                        <div className="form-group">
                            <label>Score:</label>
                            <input type="text" name="name" value={this.props.score} className="form-control" />
                        </div>
                    </div>
                </div>
            );
    }
});

var QuestionBoxEdit = React.createClass({
    render: function () {
        return (
            <div className="questionBoxEdit">
                <QuestionHeaderEdit english="English quesiton" chinese="chinese question" />
                <ChoiceEdit i={1} english="test" chinese="test" score={-2} />
                <ChoiceEdit i={2} english="test" chinese="test" score={-2} />
                <ChoiceEdit i={3} english="test" chinese="test" score={-2} />
                <div className="pull-right">
                    <button className="btn btn-default">Cancel</button>
                    <button className="btn btn-primary">Save</button>
                </div>
            </div>
            );
    }
});

var QuestionList = React.createClass({
    render: function () {
        var quesNodes = this.props.data.map(function (q) {
            return (
                <tr>
                    <td>
                        <QuestionHeader english={q.quesEnglish} chinese={q.quesChinese} trait={q.trait} />
                    </td>
                </tr>
                );
});
return (
        <table className="table">
            <tbody>
                {quesNodes}
            </tbody>
        </table>
            );
}
});

var data = [{
    quesEnglish: "What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3]
},
{
    quesEnglish: "What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3]
},
{
    quesEnglish: "What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3]
},
{
    quesEnglish: "What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3]
},
{
    quesEnglish: "What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3]
}];

ReactDOM.render(
    <QuestionList data={data} />,
    $('#content')[0]
);