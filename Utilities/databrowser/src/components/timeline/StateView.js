import React from 'react';
import Divider from '@material-ui/core/Divider';
import { withStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';

class StateView extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {
    return  <div>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Key</TableCell>
            <TableCell>Value</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {this.props.items.map(n => {
          return (
            <TableRow key={n.key + n.entity}>
              <TableCell component="th" scope="row">{n.key}</TableCell>
              <TableCell>{n.value}</TableCell>
            </TableRow>
          );
        })}
        </TableBody>
      </Table>
    </div>
  }
}

export default (StateView);
