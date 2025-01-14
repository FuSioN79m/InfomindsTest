import {
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography,
    styled,
    tableCellClasses,
  } from "@mui/material";
  import { useEffect, useState } from "react";
  
  
  interface EmployerListQuery {    
    id: number;
    code: string;
    firstName: string;
    lastName: string;
    address: string;
    email: string;
    phone: string;
    department: {
    id: number;
    code: string;
    description: string;
    }

  }

export default function EmployeeListPage() {
  const [list, setList] = useState<EmployerListQuery[]>([]);

  useEffect(() => {
    fetch("/api/employees/list")
      .then((response) => {
        return response.json();
      })
      .then((data) => {
        setList(data as EmployerListQuery[]);
      });
  }, []);
  return (
      <>
        <Typography variant="h4" sx={{ textAlign: "center", mt: 4, mb: 4 }}>
          Employees
        </Typography>
  
        <TableContainer component={Paper}>
          <Table sx={{ minWidth: 650 }} aria-label="simple table">
            <TableHead>
              <TableRow>
                <StyledTableHeadCell>code</StyledTableHeadCell>
                <StyledTableHeadCell>firstName</StyledTableHeadCell>
                <StyledTableHeadCell>lastName</StyledTableHeadCell>
                <StyledTableHeadCell>email</StyledTableHeadCell>
                <StyledTableHeadCell>dep. code</StyledTableHeadCell>
                <StyledTableHeadCell>dep. description</StyledTableHeadCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {list.map((row) => (
                <TableRow
                  key={row.id}
                  sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
                >
                  <TableCell>{row.code}</TableCell>
                  <TableCell>{row.firstName}</TableCell>
                  <TableCell>{row.lastName}</TableCell>
                  <TableCell>{row.email}</TableCell>
                  <TableCell>{row.department?.code}</TableCell>
                  <TableCell>{row.department?.description}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </>
    );
  }

  const StyledTableHeadCell = styled(TableCell)(({ theme }) => ({
    [`&.${tableCellClasses.head}`]: {
      backgroundColor: theme.palette.primary.light,
      color: theme.palette.common.white,
    },
  }));
  