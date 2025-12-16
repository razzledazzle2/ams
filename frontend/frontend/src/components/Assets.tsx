import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getAccessToken } from "../utils/auth";
import { NavigationBar } from "./NavigationBar";
import { AddAssetDialog } from "./Dialog";
import { AssetActions } from "./AssetActions";
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  ColumnDef,
  SortingState,
  getSortedRowModel,
} from "@tanstack/react-table";
import { Icon, SortAscIcon, ArrowUpDown } from "lucide-react";

const API_BASE = "http://localhost:5051";

type Asset = {
  id: string;
  name: string;
  category: string;
  status: "Available" | "Assigned" | "Maintenance";
  vendor: string;
  condition: string;
  purchaseDate: string;
  createdAt: string;
};

export const Assets = () => {
  const [addOpen, setAddOpen] = useState(false);

  const [assets, setAssets] = useState<Asset[]>([]);
  const [loading, setLoading] = useState(true);
  const [sorting, setSorting] = useState<SortingState>([]);
  const fetchAssets = async () => {
    try {
      const res = await fetch(`${API_BASE}/api/assets`, {
        headers: {
          Authorization: `Bearer ${getAccessToken()}`,
        },
      });

      if (!res.ok) throw new Error("Failed to fetch assets");

      const data = await res.json();
      setAssets(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    fetchAssets();
  }, []);

  const columns: ColumnDef<Asset>[] = [
    {
      accessorKey: "name",
      header: "Name",
    },
    {
      accessorKey: "category",
      header: "Category",
    },
    {
      accessorKey: "vendor",
      header: "Vendor",
    },
    {
      accessorKey: "condition",
      header: "Condition",
    },
    {
      accessorKey: "status",
      header: "Status",
    },
    {
      accessorKey: "purchaseDate",
      header: "Purchase Date",
      cell: ({ getValue }) => {
        const value = getValue<string>();
        return value ? new Date(value).toLocaleDateString() : "—";
      },
    },
    {
      id: "actions",
      header: "Actions",
      cell: ({ row }) => (
        <AssetActions asset={row.original} onUpdated={fetchAssets} />
      ),
    },
  ];
  const table = useReactTable({
    data: assets,
    columns,
    state: { sorting },
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  });
  return (
    <div>
      <NavigationBar
        title="Assets"
        onAdd={() => setAddOpen(true)}
      ></NavigationBar>
      <AddAssetDialog
        open={addOpen}
        onOpenChange={setAddOpen}
        onAssetCreated={fetchAssets}
      />

      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => (
                <TableHead key={header.id}>
                  <div className="flex items-center gap-2">
                    {flexRender(
                      header.column.columnDef.header,
                      header.getContext()
                    )}

                    {header.column.getCanSort() && (
                      <button
                        onClick={header.column.getToggleSortingHandler()}
                        className="text-muted-foreground hover:text-foreground"
                      >
                        {header.column.getIsSorted() === "asc" && "▲"}
                        {header.column.getIsSorted() === "desc" && "▼"}
                        {!header.column.getIsSorted() && (
                          <ArrowUpDown size={14} />
                        )}
                      </button>
                    )}
                  </div>
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>

        <TableBody>
          {table.getRowModel().rows.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow key={row.id}>
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="text-center">
                No assets found.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
};
