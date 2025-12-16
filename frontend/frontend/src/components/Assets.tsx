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
  // const [showModal, setShowModal] = useState(false);
  const [assets, setAssets] = useState<Asset[]>([]);
  const [loading, setLoading] = useState(true);
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

  console.log("assets", assets);
  return (
    <div>
      <NavigationBar isBackButton={false} title="Assets"></NavigationBar>
      <AddAssetDialog onAssetCreated={fetchAssets} />

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Category</TableHead>
            <TableHead>Vendor</TableHead>
            <TableHead>Condition</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Purchase Date</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>

        <TableBody>
          {assets.map((asset) => (
            <TableRow key={asset.id}>
              <TableCell>{asset.name}</TableCell>
              <TableCell>{asset.category}</TableCell>
              <TableCell>{asset.vendor}</TableCell>
              <TableCell>{asset.condition}</TableCell>
              <TableCell>{asset.status}</TableCell>
              <TableCell>
                {asset.purchaseDate
                  ? new Date(asset.purchaseDate).toLocaleDateString()
                  : "—"}
              </TableCell>
              <TableCell>
                <AssetActions asset={asset} onUpdated={fetchAssets} />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};
