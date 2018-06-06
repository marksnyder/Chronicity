import React from 'react';
import { render } from 'react-dom';
import brace from 'brace';
import AceEditor from 'react-ace';
import 'brace/mode/javascript';
import 'brace/theme/github';
import Divider from '@material-ui/core/Divider';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import AppBar from '@material-ui/core/AppBar';
import Button from '@material-ui/core/Button';
import Toolbar from '@material-ui/core/Toolbar';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import Chronicity from '../../clients/Chronicity.js'

class CodeEditor extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      value: 0,
      codeText: JSON.stringify(this.props.filters, null, 2)
    };
  }

  changeCodeText = (codeText) => {
    this.setState({
      codeText: codeText
    });
  }

  runCode = () => {
    console.log('parsing json');
    this.evalCode.call(this);
    console.log('parsed json');
  }

  evalCode = (code) => {
      eval(this.state.codeText);
  }

  getClient = () => {
    return Chronicity;
  }

  render() {
    const { classes } = this.props;
    const { value } = this.state;

    return  <div className={classes.root}>
    <AppBar color="secondary" position="static">
        <Toolbar>
          <Typography variant="subheading" color="inherit" className={classes.flex}>
          Filter Query
        </Typography>
          <IconButton size="small" aria-label="Play/pause">
            <PlayArrowIcon onClick={this.runCode} className={classes.playIcon} />
          </IconButton>
        </Toolbar>
      </AppBar>
      <Paper className={classes.paper}>
        <AceEditor
          mode="javascript"
          theme="github"
          name="filterEditor"
          fontSize={14}
          showPrintMargin={true}
          showGutter={true}
          highlightActiveLine={true}
          value={this.state.codeText}
          onChange={this.changeCodeText}
        />
      </Paper>
    </div>
  }
}

export default (CodeEditor);
