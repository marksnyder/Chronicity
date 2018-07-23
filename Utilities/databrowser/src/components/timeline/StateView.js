import React from 'react';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';

class StateView extends React.Component {

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
              <TableCell>
                {n.key == 'cam' &&
                <img src={'http://viewex.chronicity.io/images/' + n.value} style={{ width: 400, padding: 5 }} alt="Cam Image" />
                }
                <div>{n.value}</div>
              </TableCell>
            </TableRow>
          );
        })}
        </TableBody>
      </Table>
    </div>
  }
}

export default (StateView);
