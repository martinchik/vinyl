CREATE OR REPLACE FUNCTION make_tsvector(TextLine1 TEXT, TextLine2 TEXT)
   RETURNS tsvector AS $$
BEGIN
  RETURN (setweight(to_tsvector('simple', TextLine1),'A') || setweight(to_tsvector('english', TextLine2), 'B'));
END
$$ LANGUAGE 'plpgsql' IMMUTABLE;

CREATE INDEX IF NOT EXISTS idx_fts_resords ON "SearchItem"
  USING gin(make_tsvector("TextLine1", "TextLine2"));
  
CREATE OR REPLACE FUNCTION fts_search(searchQuery TEXT)
    RETURNS TABLE("PriceFrom" numeric, "PriceTo" numeric, "RecordId" uuid, "Sell" boolean, "TextLine1" TEXT, "TextLine2" TEXT) 
AS 
$$ SELECT "PriceFrom", "PriceTo", "RecordId", "Sell", "TextLine1", "TextLine2"
        FROM "SearchItem", to_tsquery(searchQuery) as q 
        WHERE make_tsvector("TextLine1", "TextLine2") @@ q
        ORDER BY ts_rank(make_tsvector("TextLine1", "TextLine2"), q) DESC
$$ 
LANGUAGE SQL; 


