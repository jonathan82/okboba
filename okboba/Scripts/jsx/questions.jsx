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
                    <div className="form-inline">
                        <div className="form-group">
                            <label>Choice {this.props.i+1}</label>
                            <input type="text" name="name" value={this.props.english} className="form-control" size="30"/>
                            <input type="text" name="name" className="form-control" value={this.props.chinese} size="30"/>
                        </div>
                        <div className="form-group">
                            <label>Score:</label>
                            <input type="text" name="name" value={this.props.score} className="form-control" size="3"/>
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

var PageNav = React.createClass({
    handleClick: function (e) {
        e.preventDefault();
        var page = $(e.currentTarget).data('page');
        this.props.pageClickHandler(page);
    },
    generateLinks: function (page, numPages) {
        var liNodes = [];
        var active = '';
        for (var i = 1; i <= numPages; i++) {
            active = i == page ? 'active' : '';
            liNodes.push(<li key={i} className={active}><a href='#' onClick={this.handleClick} data-page={i}>{i}</a></li>);
        }
        return liNodes;
    },
    render: function () {
        if (!this.props.numPages) return null;
        var liNodes = this.generateLinks(this.props.page, this.props.numPages);

        return (<nav>
              <ul className="pagination">
                {liNodes}
              </ul>
            </nav>);
    }
});

var QuestionList = React.createClass({
    pageClick: function (page) {
        this.setState({ page: page, loading: true });
        this.loadQuestionsFromServer(page);
    },
    loadQuestionsFromServer: function (page) {
        var myComponent = this;
        var param = page ? 'page=' + page : '';
        param += this.props.pageSize ? '&pageSize=' + this.props.pageSize : '';
        $.get(this.props.url, param, function (data, status, jqxhr) {
            myComponent.setState({data: data.Questions, loading: false, numPages: data.PageCount})
        });
    },
    getInitialState: function () {
        return {data:[], loading: true, page: 1};
    },
    componentDidMount: function () {
        this.loadQuestionsFromServer();
    },
    render: function () {
        var quesNodes = this.state.data.map(function (q) {
            return (
                <tr className="question-row" key={q.Id}>
                    <td>
                        <QuestionBox q={q} />
                    </td>
                </tr>
                );
        });
        return (
            <div>    
                {this.state.loading ? <h3>Loading...</h3> : ''}
                <center>
                <PageNav page={this.state.page} numPages={this.state.numPages} pageClickHandler={this.pageClick} />
                </center>
                <table className="table">
                    <tbody id="questionList">
                        {quesNodes}
                    </tbody>
                </table>
                <center>
                <PageNav page={this.state.page} numPages={this.state.numPages} pageClickHandler={this.pageClick} />
                </center>
             </div>    
                );
    }
});


ReactDOM.render(
    <QuestionList url="/admin/gettranslatequestions" pageSize={75} />,
    $('#content')[0]
);