import React from 'react';
import { render } from 'react-dom';
import brace from 'brace';
import AceEditor from 'react-ace';
import 'brace/mode/javascript';
import 'brace/theme/tomorrow';
import Divider from '@material-ui/core/Divider';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import AppBar from '@material-ui/core/AppBar';
import Button from '@material-ui/core/Button';
import Toolbar from '@material-ui/core/Toolbar';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import SaveIcon from '@material-ui/icons/Save';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';

class CodeView extends React.Component {

  constructor(props) {
    super(props);
  }


  saveCode = () => {
    this.props.saveCode();
  }

  runCode = () => {
    this.props.runCode();
  };

  render() {

      const { classes } = this.props;

      return  (<div className={classes.root}>
        <AppBar color="secondary" position="static">
            <Toolbar>
              <Typography variant="subheading" color="inherit" className={classes.flex}>
              Code?
            </Typography>
              <IconButton size="small" aria-label="Play/pause">
                <SaveIcon onClick={this.props.saveCode} className={classes.saveIcon} />
              </IconButton>
              <IconButton size="small" aria-label="Play/pause">
                <PlayArrowIcon onClick={this.props.runCode} className={classes.playIcon} />
              </IconButton>
            </Toolbar>
          </AppBar>
          <Paper className={classes.paper}>
            <AceEditor
              mode="javascript"
              theme="tomorrow"
              name="filterEditor"
              fontSize={14}
              showPrintMargin={true}
              showGutter={true}
              highlightActiveLine={true}
              value={this.props.initialCode}
              onChange={this.props.setCode}
              width="100%"
              height="5000px"
            />
          </Paper>
        </div>);
    };
}

export default (CodeView);
