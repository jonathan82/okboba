var QuestionHeaderEdit = React.createClass({
    render: function () {
        return (
            <div className="questionHeaderEdit">
                <div className="row">
                    <div className="form-group col-md-9">
                        <input type="text" name="name" value={this.props.q.QuesEng} className="form-control" />
                    </div>
                    <div className="col-md-3 form-group">
                        <select className="form-control">
                            <option value="value">text</option>
                        </select>
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-9 form-group">
                        <input type="text" name="name" defaultValue={this.props.q.QuesChin} className="form-control" />
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
            <div onClick={this.props.headerClick}>
                <div className="row">
                    <div className="col-sm-9">
                        {this.props.q.QuesEng}
                    </div>
                    <div className="col-sm-3">
                        <b>{this.props.q.Trait}</b>
                    </div>
                </div>
                <div className="row">
                    <div className="col-sm-12">
                        {this.props.q.QuesChin}
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
        var props = this.props;
        var choiceNodes = this.props.q.ChoicesEng.map(function (choiceEng, index) {

            var english = choiceEng;
            var chinese = props.q.ChoicesChin == null ? null : props.q.ChoicesChin[index];
            var score = props.q.Scores == null ? null : props.q.Scores[index];

            return <ChoiceEdit i={index} english={english} chinese={chinese} score={score} />
        });
        return (
            <form action="">
                <QuestionHeaderEdit q={this.props.q} />
                {choiceNodes}
                <div className="pull-right">
                    <button type="button" className="btn btn-default" onClick={this.props.onQuestionCancel}>Cancel</button>
                    <button type="button" className="btn btn-primary">Save</button>
                </div>
            </form>
            );
    }
});

var QuestionBox = React.createClass({
    handleCancel: function () {
        this.setState({ isEditing: false });
    },
    handleClick: function () {       
        this.setState({isEditing: true});
    },
    getInitialState: function () {
        return { isEditing: false };
    },
    render: function () {
        if (this.state.isEditing) {
            return <QuestionBoxEdit q={this.props.q} onQuestionCancel={this.handleCancel} />;
        } else {
            return <QuestionHeader q={this.props.q} headerClick={this.handleClick} />;
        }
    }
});

var QuestionList = React.createClass({
    loadQuestionsFromServer: function (url) {
        var myComponent = this;
        $.get(url, '', function (data, status, jqxhr) {
            myComponent.setState({data: data})
        });
    },
    getInitialState: function () {
        return {data:[]};
    },
    componentDidMount: function () {
        this.loadQuestionsFromServer(this.props.url);
    },
    render: function () {
        var quesNodes = this.state.data.map(function (q) {
            return (
                <tr className="question-row">
                    <td>
                        <QuestionBox q={q} />
                    </td>
                </tr>
                );
        });
        return (
                <table className="table">
                    <tbody id="questionList">
                        {quesNodes}
                    </tbody>
                </table>
                    );
    }
});

var data = [{
    quesEnglish: "1What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, null],
},
{
    quesEnglish: "2What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3],
},
{
    quesEnglish: "3What is your favorite color?",
    quesChinese: "什么是你的最喜欢的颜色？",
    trait: "Kinkiness",
    choicesEnglish: ['Blue', 'Yellow', 'Green'],
    choicesChinese: ['兰', '率', '红'],
    scores: [1, 2, 3],
}];

ReactDOM.render(
    <QuestionList data={data} url="/admin/gettranslatequestions" />,
    $('#content')[0]
);